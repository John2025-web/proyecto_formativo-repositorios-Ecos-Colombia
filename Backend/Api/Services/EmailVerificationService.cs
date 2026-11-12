using System.Net;
using System.Net.Mail;

namespace ApiPrueba.Services;

public sealed class EmailVerificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailVerificationService> _logger;

    public EmailVerificationService(IConfiguration configuration, ILogger<EmailVerificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(bool SentBySmtp, string? ErrorMessage)> SendVerificationCodeAsync(
        string recipientEmail,
        string recipientName,
        string code,
        CancellationToken cancellationToken = default)
    {
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _configuration["Email:SmtpHost"];
        var smtpPortRaw = Environment.GetEnvironmentVariable("SMTP_PORT") ?? _configuration["Email:SmtpPort"];
        var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? _configuration["Email:SmtpUser"];
        var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? _configuration["Email:SmtpPass"];
        var fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM") ?? _configuration["Email:FromEmail"] ?? smtpUser;
        var fromName = _configuration["Email:FromName"] ?? "Ecos Colombia";

        if (string.IsNullOrWhiteSpace(smtpHost)
            || string.IsNullOrWhiteSpace(smtpUser)
            || string.IsNullOrWhiteSpace(smtpPass)
            || string.IsNullOrWhiteSpace(fromEmail))
        {
            _logger.LogInformation(
                "SMTP no configurado. Código de verificación generado para {Correo}: {Codigo}",
                recipientEmail,
                code);
            return (false, "SMTP no configurado; se habilitó el código de verificación directo.");
        }

        var port = int.TryParse(smtpPortRaw, out var parsedPort) ? parsedPort : 587;

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = $"Código de verificación: {code} - Ecos Colombia",
                IsBodyHtml = true,
                Body = $"""
                    <div style="font-family: 'Segoe UI', Arial, sans-serif; max-width: 520px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; overflow: hidden;">
                      <div style="background: #243c8f; padding: 24px; text-align: center; border-bottom: 4px solid #f4d35e;">
                        <h1 style="margin: 0; color: #ffffff; font-size: 22px;">Ecos <span style="color: #f4d35e;">Colombia</span></h1>
                      </div>
                      <div style="padding: 28px 24px; color: #1e293b;">
                        <p style="margin-top: 0; font-size: 15px;">Hola <strong>{WebUtility.HtmlEncode(recipientName)}</strong>,</p>
                        <p style="font-size: 14px; line-height: 1.6; color: #475569;">
                          Recibimos una solicitud para iniciar sesión con tu correo electrónico <strong>{WebUtility.HtmlEncode(recipientEmail)}</strong>.
                          Ingresa el siguiente código de verificación para confirmar tu acceso:
                        </p>
                        <div style="margin: 24px 0; padding: 18px; background: #f8fafc; border: 2px dashed #243c8f; border-radius: 12px; text-align: center;">
                          <span style="font-size: 32px; font-weight: 700; letter-spacing: 8px; color: #243c8f; font-family: monospace;">{WebUtility.HtmlEncode(code)}</span>
                        </div>
                        <p style="font-size: 12px; color: #64748b; margin-bottom: 0;">
                          Este código expira en 10 minutos. Si no solicitaste este código, puedes ignorar este mensaje.
                        </p>
                      </div>
                    </div>
                    """
            };
            message.To.Add(new MailAddress(recipientEmail, recipientName));

            using var client = new SmtpClient(smtpHost, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Correo de verificación enviado por SMTP a {Correo}.", recipientEmail);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No fue posible enviar el correo por SMTP a {Correo}.", recipientEmail);
            return (false, ex.Message);
        }
    }
}
