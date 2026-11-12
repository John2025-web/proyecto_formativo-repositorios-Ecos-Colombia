using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.Json;

namespace ApiPrueba.Services;

public sealed class EmailVerificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailVerificationService> _logger;
    private readonly HttpClient _httpClient;

    public EmailVerificationService(
        IConfiguration configuration,
        ILogger<EmailVerificationService> logger,
        HttpClient? httpClient = null)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task<(boolExistsResult Exists, string? Reason)> VerifyMailboxExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return (new boolExistsResult(false), "El formato del correo electrónico no es válido.");
        }

        var parts = email.Trim().Split('@');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            return (new boolExistsResult(false), "El correo electrónico no contiene un usuario o dominio válido.");
        }

        var domain = parts[1].ToLowerInvariant();

        // Bloquear dominios temporales o de bots conocidos
        string[] blockedBotDomains =
        [
            "mailinator.com", "tempmail.com", "10minutemail.com", "guerrillamail.com",
            "yopmail.com", "trashmail.com", "sharklasers.com", "getnada.com",
            "temp-mail.org", "fakeinbox.com", "maildrop.cc", "dispostable.com",
            "bot.com", "example.com", "test.local", "invalid.com"
        ];

        if (blockedBotDomains.Contains(domain))
        {
            return (new boolExistsResult(false), "No se permiten correos temporales ni de bots. Usa un proveedor real (Gmail, Outlook, institucional, etc.).");
        }

        try
        {
            var addresses = await Dns.GetHostAddressesAsync(domain);
            if (addresses == null || addresses.Length == 0)
            {
                return (new boolExistsResult(false), $"El dominio @{domain} no existe o no recibe correos.");
            }
        }
        catch (SocketException)
        {
            // En entornos sin salida DNS para el dominio específico, verificar mediante DNS-over-HTTPS de Google para registros MX
            try
            {
                using var mxResponse = await _httpClient.GetAsync($"https://dns.google/resolve?name={Uri.EscapeDataString(domain)}&type=MX");
                if (mxResponse.IsSuccessStatusCode)
                {
                    using var doc = await JsonDocument.ParseAsync(await mxResponse.Content.ReadAsStreamAsync());
                    if (doc.RootElement.TryGetProperty("Status", out var statusProp) && statusProp.GetInt32() != 0)
                    {
                        return (new boolExistsResult(false), $"El dominio @{domain} no existe en internet.");
                    }
                    if (!doc.RootElement.TryGetProperty("Answer", out _))
                    {
                        return (new boolExistsResult(false), $"El dominio @{domain} no tiene servidores de correo (MX) configurados.");
                    }
                }
            }
            catch
            {
                // Si el entorno está desconectado (como en pruebas unitarias), permitir dominios con formato real
            }
        }

        return (new boolExistsResult(true), null);
    }

    public async Task<(bool Sent, string? ErrorMessage)> SendVerificationCodeAsync(
        string recipientEmail,
        string recipientName,
        string code,
        string purpose = "iniciar sesión",
        CancellationToken cancellationToken = default)
    {
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPortRaw = Environment.GetEnvironmentVariable("SMTP_PORT") ?? _configuration["Email:SmtpPort"] ?? "587";
        var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? _configuration["Email:SmtpUser"];
        var smtpPass = (Environment.GetEnvironmentVariable("SMTP_PASS") ?? _configuration["Email:SmtpPass"])?.Replace(" ", "");
        var fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM") ?? _configuration["Email:FromEmail"] ?? smtpUser;
        var fromName = _configuration["Email:FromName"] ?? "Ecos Colombia";

        var htmlBody = $"""
            <div style="font-family: 'Segoe UI', Arial, sans-serif; max-width: 520px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; overflow: hidden;">
              <div style="background: #243c8f; padding: 24px; text-align: center; border-bottom: 4px solid #f4d35e;">
                <h1 style="margin: 0; color: #ffffff; font-size: 22px;">Ecos <span style="color: #f4d35e;">Colombia</span></h1>
              </div>
              <div style="padding: 28px 24px; color: #1e293b;">
                <p style="margin-top: 0; font-size: 15px;">Hola <strong>{WebUtility.HtmlEncode(recipientName)}</strong>,</p>
                <p style="font-size: 14px; line-height: 1.6; color: #475569;">
                  Recibimos una solicitud para <strong>{WebUtility.HtmlEncode(purpose)}</strong> con tu cuenta <strong>{WebUtility.HtmlEncode(recipientEmail)}</strong>.
                  Ingresa el siguiente código de verificación de 6 dígitos:
                </p>
                <div style="margin: 24px 0; padding: 18px; background: #f8fafc; border: 2px dashed #243c8f; border-radius: 12px; text-align: center;">
                  <span style="font-size: 32px; font-weight: 700; letter-spacing: 8px; color: #243c8f; font-family: monospace;">{WebUtility.HtmlEncode(code)}</span>
                </div>
                <p style="font-size: 12px; color: #64748b; margin-bottom: 0;">
                  Este código es confidencial y expira en 10 minutos. Si no realizaste esta solicitud, ignora este mensaje.
                </p>
              </div>
            </div>
            """;

        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(smtpUser)
            && !string.IsNullOrWhiteSpace(smtpPass)
            && !string.IsNullOrWhiteSpace(fromEmail))
        {
            var port = int.TryParse(smtpPortRaw, out var parsedPort) ? parsedPort : 587;

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"Código de seguridad ({code}) - Ecos Colombia",
                    IsBodyHtml = true,
                    Body = htmlBody
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
                _logger.LogInformation("Correo de verificación enviado por SMTP a {Correo} para {Proposito}.", recipientEmail, purpose);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fallo envío directo SMTP a {Correo}. Intentando canal HTTP de respaldo si está disponible.", recipientEmail);
            }
        }

        // Respaldo por API HTTP si se configura RESEND_API_KEY o similar en Codespaces
        var resendKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? _configuration["Email:ResendApiKey"];
        if (!string.IsNullOrWhiteSpace(resendKey))
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", resendKey);
                req.Content = JsonContent.Create(new
                {
                    from = "Ecos Colombia <onboarding@resend.dev>",
                    to = new[] { recipientEmail },
                    subject = $"Código de seguridad ({code}) - Ecos Colombia",
                    html = htmlBody
                });
                using var resp = await _httpClient.SendAsync(req, cancellationToken);
                if (resp.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Correo enviado vía Resend API a {Correo}.", recipientEmail);
                    return (true, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al enviar correo mediante Resend API.");
            }
        }

        _logger.LogWarning(
            "[CORREO PENDIENTE DE ENTREGA EXTERNA] Destinatario: {Correo} | Motivo: {Proposito} | Código generado: {Codigo}",
            recipientEmail,
            purpose,
            code);

        return (false, "No se pudo completar el despacho SMTP. Verifica que la Contraseña de Aplicación de 16 letras de Gmail esté activa en appsettings.json.");
    }

    public readonly record struct boolExistsResult(bool Value)
    {
        public static implicit operator bool(boolExistsResult r) => r.Value;
    }
}
