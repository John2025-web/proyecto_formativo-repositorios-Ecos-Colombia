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
                // En entornos desconectados, permitir dominios con formato real
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
        var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? _configuration["Email:SmtpUser"] ?? "ecos.colombia1@gmail.com";
        var rawPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? _configuration["Email:SmtpPass"];
        if (string.IsNullOrWhiteSpace(rawPass))
        {
            var token = _configuration["Email:SmtpSecretToken"];
            if (!string.IsNullOrWhiteSpace(token))
            {
                rawPass = UnprotectInternalSecret(token);
            }
        }
        var smtpPass = rawPass?.Replace(" ", "");
        var fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM") ?? _configuration["Email:FromEmail"] ?? "ecos.colombia1@gmail.com";
        var fromName = _configuration["Email:FromName"] ?? "Ecos Colombia";

        var htmlBody = $"""
            <div style="font-family: 'Segoe UI', Arial, sans-serif; max-width: 520px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; overflow: hidden;">
              <div style="background: #243c8f; padding: 24px; text-align: center; border-bottom: 4px solid #f4d35e;">
                <h1 style="margin: 0; color: #ffffff; font-size: 22px;">Ecos <span style="color: #f4d35e;">Colombia</span></h1>
                <p style="margin: 6px 0 0; color: #e2e8f0; font-size: 12px;">Remitente oficial: {WebUtility.HtmlEncode(fromEmail)}</p>
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
                  Enviado por <strong>{WebUtility.HtmlEncode(fromEmail)}</strong>. Este código es confidencial y expira en 10 minutos. Si no realizaste esta solicitud, ignora este mensaje.
                </p>
              </div>
            </div>
            """;

        var port = int.TryParse(smtpPortRaw, out var parsedPort) ? parsedPort : 587;

        // 1. Intentar envío con el correo remitente principal configurado (ecos.colombia1@gmail.com / Aa123456*)
        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(smtpUser)
            && !string.IsNullOrWhiteSpace(smtpPass)
            && !string.IsNullOrWhiteSpace(fromEmail))
        {
            if (await TrySendSmtpMessageAsync(smtpHost, port, smtpUser, smtpPass, fromEmail, fromName, recipientEmail, recipientName, code, htmlBody, cancellationToken))
            {
                _logger.LogInformation("Correo enviado por SMTP desde {Remitente} a {Correo} para {Proposito}.", fromEmail, recipientEmail, purpose);
                return (true, null);
            }
        }

        // 2. Si Gmail requiere App Password en el canal principal, usar el transporte SMTP autenticado de respaldo manteniendo a ecos.colombia1@gmail.com como remitente y ReplyTo
        var fallbackUser = _configuration["Email:FallbackSmtpUser"];
        var fallbackToken = _configuration["Email:FallbackSmtpSecretToken"];
        var fallbackPass = !string.IsNullOrWhiteSpace(fallbackToken) ? UnprotectInternalSecret(fallbackToken) : null;

        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(fallbackUser)
            && !string.IsNullOrWhiteSpace(fallbackPass))
        {
            if (await TrySendSmtpMessageAsync(smtpHost, port, fallbackUser, fallbackPass, fromEmail, fromName, recipientEmail, recipientName, code, htmlBody, cancellationToken))
            {
                _logger.LogInformation("Correo enviado para {Proposito} a {Correo} con remitente {Remitente}.", purpose, recipientEmail, fromEmail);
                return (true, null);
            }
        }

        _logger.LogWarning(
            "[CORREO PENDIENTE DE ENTREGA EXTERNA] Remitente: {Remitente} | Destinatario: {Correo} | Motivo: {Proposito}",
            fromEmail,
            recipientEmail,
            purpose);

        return (false, "No se pudo completar el despacho SMTP.");
    }

    private static async Task<bool> TrySendSmtpMessageAsync(
        string smtpHost,
        int port,
        string authUser,
        string authPass,
        string fromEmail,
        string fromName,
        string recipientEmail,
        string recipientName,
        string code,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Sender = new MailAddress(fromEmail, fromName),
                Subject = $"Código de seguridad ({code}) - Ecos Colombia",
                IsBodyHtml = true,
                Body = htmlBody
            };
            message.ReplyToList.Add(new MailAddress(fromEmail, fromName));
            message.To.Add(new MailAddress(recipientEmail, recipientName));

            using var client = new SmtpClient(smtpHost, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(authUser, authPass),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            await client.SendMailAsync(message, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string UnprotectInternalSecret(string encoded)
    {
        try
        {
            var hex = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var chars = new char[hex.Length / 2];
            for (int i = 0; i < chars.Length; i++)
            {
                var b = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                chars[i] = (char)(b ^ 0x16);
            }
            return new string(chars);
        }
        catch
        {
            return string.Empty;
        }
    }

    public readonly record struct boolExistsResult(bool Value)
    {
        public static implicit operator bool(boolExistsResult r) => r.Value;
    }
}
