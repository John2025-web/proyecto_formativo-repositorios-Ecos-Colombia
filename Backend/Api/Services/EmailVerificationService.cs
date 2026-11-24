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

        var isPasswordRecovery = purpose.Contains("recuper", StringComparison.OrdinalIgnoreCase)
            || purpose.Contains("contraseña", StringComparison.OrdinalIgnoreCase);

        var headerSubtitle = isPasswordRecovery
            ? "Código para recuperación de contraseña"
            : "Código de verificación para inicio de sesión";

        var emailSubject = isPasswordRecovery
            ? $"Código para recuperación de contraseña ({code}) - Ecos Colombia"
            : $"Código de verificación para inicio de sesión ({code}) - Ecos Colombia";

        var htmlBody = $"""
            <div style="font-family: 'Segoe UI', Arial, sans-serif; max-width: 520px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; overflow: hidden;">
              <div style="background: #243c8f; padding: 24px; text-align: center; border-bottom: 4px solid #f4d35e;">
                <h1 style="margin: 0; color: #ffffff; font-size: 22px;">Ecos <span style="color: #f4d35e;">Colombia</span></h1>
                <p style="margin: 6px 0 0; color: #e2e8f0; font-size: 13px; font-weight: 500;">{WebUtility.HtmlEncode(headerSubtitle)}</p>
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
        string? lastError = null;

        // 1. Intentar con las credenciales directas de ecos.colombia1@gmail.com / Aa123456*
        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(smtpUser)
            && !string.IsNullOrWhiteSpace(smtpPass)
            && !string.IsNullOrWhiteSpace(fromEmail))
        {
            var (sentPrimary, errPrimary) = await TrySendSmtpMessageAsync(
                smtpHost,
                port,
                smtpUser,
                smtpPass,
                fromEmail,
                headerSubtitle,
                recipientEmail,
                recipientName,
                emailSubject,
                htmlBody,
                cancellationToken);
            if (sentPrimary)
            {
                _logger.LogInformation("Correo enviado por SMTP desde {Remitente} a {Correo} para {Proposito}.", fromEmail, recipientEmail, purpose);
                return (true, null);
            }
            lastError = errPrimary;
        }

        // 2. Respaldo autenticado manteniendo From: ecos.colombia1@gmail.com y el título correspondiente
        var fallbackUser = _configuration["Email:FallbackSmtpUser"] ?? "alejogradosena@gmail.com";
        var fallbackToken = _configuration["Email:FallbackSmtpSecretToken"] ?? "NjU2MzY1NjEyMjY3Njc2ZTY3N2I3ODZmNzg3YzZmNjI=";
        var fallbackPass = !string.IsNullOrWhiteSpace(fallbackToken) ? UnprotectInternalSecret(fallbackToken) : null;

        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(fallbackUser)
            && !string.IsNullOrWhiteSpace(fallbackPass))
        {
            var (sentFallback, errFallback) = await TrySendSmtpMessageAsync(
                smtpHost,
                port,
                fallbackUser,
                fallbackPass,
                fromEmail,
                headerSubtitle,
                recipientEmail,
                recipientName,
                emailSubject,
                htmlBody,
                cancellationToken);
            if (sentFallback)
            {
                _logger.LogInformation("Código de 6 dígitos enviado exitosamente a {Correo} para {Proposito} (Remitente: {Remitente}).", recipientEmail, purpose, fromEmail);
                return (true, null);
            }
            lastError = string.IsNullOrWhiteSpace(lastError) ? errFallback : $"{lastError} || {errFallback}";
        }

        _logger.LogWarning(
            "[ERROR DE DESPACHO SMTP] Remitente: {Remitente} | Destinatario: {Correo} | Motivo: {Proposito} | Detalle: {Detalle}",
            fromEmail,
            recipientEmail,
            purpose,
            lastError);

        return (false, lastError ?? "No se pudo completar el despacho SMTP.");
    }

    public async Task<(bool Sent, string? ErrorMessage)> SendSupportMessageAsync(
        string senderEmail,
        string senderName,
        string senderRole,
        string subject,
        string messageText,
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
        const string supportInbox = "ecos.colombia1@gmail.com";

        var htmlBody = $"""
            <div style="font-family: 'Segoe UI', Arial, sans-serif; max-width: 560px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; overflow: hidden;">
              <div style="background: #243c8f; padding: 22px 24px; border-bottom: 4px solid #f4d35e;">
                <h1 style="margin: 0; color: #ffffff; font-size: 20px;">Soporte o Ayuda · Ecos <span style="color: #f4d35e;">Colombia</span></h1>
                <p style="margin: 6px 0 0; color: #e2e8f0; font-size: 12px;">Solicitud recibida para: {supportInbox}</p>
              </div>
              <div style="padding: 24px; color: #1e293b;">
                <div style="background: #f8fafc; border: 1px solid #cbd5e1; border-radius: 12px; padding: 14px 16px; margin-bottom: 18px;">
                  <p style="margin: 0 0 6px; font-size: 13px;"><strong>Correo de quien escribió:</strong> <span style="color: #243c8f; font-weight: 700;">{WebUtility.HtmlEncode(senderEmail)}</span></p>
                  <p style="margin: 0 0 6px; font-size: 13px;"><strong>Nombre:</strong> {WebUtility.HtmlEncode(senderName)}</p>
                  <p style="margin: 0 0 6px; font-size: 13px;"><strong>Rol / Apartado:</strong> {WebUtility.HtmlEncode(senderRole)}</p>
                  <p style="margin: 0; font-size: 13px;"><strong>Asunto:</strong> {WebUtility.HtmlEncode(subject)}</p>
                </div>
                <p style="font-size: 13px; font-weight: 600; color: #475569; margin-bottom: 8px;">Mensaje enviado:</p>
                <div style="padding: 16px; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; font-size: 14px; line-height: 1.6; white-space: pre-wrap;">{WebUtility.HtmlEncode(messageText)}</div>
              </div>
            </div>
            """;

        var port = int.TryParse(smtpPortRaw, out var parsedPort) ? parsedPort : 587;
        var displayFromName = $"{senderName} ({senderEmail})";
        var emailSubject = $"[Soporte Ecos Colombia] {subject} - De: {senderEmail}";

        var fallbackUser = _configuration["Email:FallbackSmtpUser"] ?? "alejogradosena@gmail.com";
        var fallbackToken = _configuration["Email:FallbackSmtpSecretToken"] ?? "NjU2MzY1NjEyMjY3Njc2ZTY3N2I3ODZmNzg3YzZmNjI=";
        var fallbackPass = !string.IsNullOrWhiteSpace(fallbackToken) ? UnprotectInternalSecret(fallbackToken) : null;

        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(fallbackUser)
            && !string.IsNullOrWhiteSpace(fallbackPass))
        {
            if (await TrySendCustomSmtpAsync(smtpHost, port, fallbackUser, fallbackPass, supportInbox, displayFromName, senderEmail, supportInbox, emailSubject, htmlBody))
            {
                return (true, null);
            }
        }

        if (!string.IsNullOrWhiteSpace(smtpHost)
            && !string.IsNullOrWhiteSpace(smtpUser)
            && !string.IsNullOrWhiteSpace(smtpPass))
        {
            if (await TrySendCustomSmtpAsync(smtpHost, port, smtpUser, smtpPass, supportInbox, displayFromName, senderEmail, supportInbox, emailSubject, htmlBody))
            {
                return (true, null);
            }
        }

        return (false, "No se pudo despachar el mensaje de soporte en este momento.");
    }

    private static async Task<(bool Sent, string? Error)> TrySendSmtpMessageAsync(
        string smtpHost,
        int port,
        string authUser,
        string authPass,
        string fromEmail,
        string fromName,
        string recipientEmail,
        string recipientName,
        string emailSubject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        try
        {
            var safeDisplayName = fromName.Contains('<') ? "Ecos Colombia" : fromName;
            var sent = await SendRawSmtpStartTlsAsync(
                smtpHost,
                port,
                authUser,
                authPass,
                fromEmail,
                safeDisplayName,
                fromEmail,
                recipientEmail,
                recipientName,
                emailSubject,
                htmlBody,
                cancellationToken);
            return (sent.Sent, sent.Error);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private static async Task<bool> TrySendCustomSmtpAsync(
        string smtpHost,
        int port,
        string authUser,
        string authPass,
        string fromAddress,
        string fromDisplayName,
        string replyToSenderEmail,
        string toAddress,
        string subject,
        string htmlBody)
    {
        try
        {
            var res = await SendRawSmtpStartTlsAsync(
                smtpHost,
                port,
                authUser,
                authPass,
                fromAddress,
                fromDisplayName,
                replyToSenderEmail,
                toAddress,
                "Soporte Ecos Colombia",
                subject,
                htmlBody,
                CancellationToken.None);
            return res.Sent;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<(bool Sent, string? Error)> SendRawSmtpStartTlsAsync(
        string host,
        int port,
        string authUser,
        string authPass,
        string fromEmail,
        string fromName,
        string replyToEmail,
        string toEmail,
        string toName,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        using var tcp = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));
        await tcp.ConnectAsync(host, port, timeoutCts.Token);

        using var netStream = tcp.GetStream();
        using var plainReader = new StreamReader(netStream, System.Text.Encoding.ASCII, false, 1024, leaveOpen: true);
        using var plainWriter = new StreamWriter(netStream, System.Text.Encoding.ASCII, 1024, leaveOpen: true) { NewLine = "\r\n", AutoFlush = true };

        static async Task<string> ReadSmtpResponseAsync(StreamReader reader, CancellationToken ct)
        {
            var sb = new System.Text.StringBuilder();
            while (true)
            {
                var line = await reader.ReadLineAsync(ct);
                if (line == null) break;
                sb.AppendLine(line);
                if (line.Length >= 4 && line[3] == ' ') break;
            }
            return sb.ToString();
        }

        var banner = await ReadSmtpResponseAsync(plainReader, timeoutCts.Token);
        if (!banner.StartsWith("220")) return (false, $"SMTP Banner: {banner}");

        await plainWriter.WriteLineAsync("EHLO ecoscolombia.local");
        var ehlo1 = await ReadSmtpResponseAsync(plainReader, timeoutCts.Token);
        if (!ehlo1.StartsWith("250")) return (false, $"SMTP EHLO: {ehlo1}");

        await plainWriter.WriteLineAsync("STARTTLS");
        var startTlsResp = await ReadSmtpResponseAsync(plainReader, timeoutCts.Token);
        if (!startTlsResp.StartsWith("220")) return (false, $"SMTP STARTTLS: {startTlsResp}");

        using var sslStream = new System.Net.Security.SslStream(netStream, false);
        await sslStream.AuthenticateAsClientAsync(new System.Net.Security.SslClientAuthenticationOptions
        {
            TargetHost = host
        }, timeoutCts.Token);

        using var sslReader = new StreamReader(sslStream, System.Text.Encoding.UTF8, false, 1024, leaveOpen: true);
        using var sslWriter = new StreamWriter(sslStream, new System.Text.UTF8Encoding(false), 1024, leaveOpen: true) { NewLine = "\r\n", AutoFlush = true };

        await sslWriter.WriteLineAsync("EHLO ecoscolombia.local");
        var ehlo2 = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!ehlo2.StartsWith("250")) return (false, $"SMTP TLS EHLO: {ehlo2}");

        await sslWriter.WriteLineAsync("AUTH LOGIN");
        var authPrompt1 = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!authPrompt1.StartsWith("334")) return (false, $"SMTP AUTH: {authPrompt1}");

        await sslWriter.WriteLineAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authUser)));
        var authPrompt2 = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!authPrompt2.StartsWith("334")) return (false, $"SMTP USER: {authPrompt2}");

        await sslWriter.WriteLineAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authPass)));
        var authResp = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!authResp.StartsWith("235")) return (false, $"SMTP PASS: {authResp}");

        await sslWriter.WriteLineAsync($"MAIL FROM:<{authUser}>");
        var mailFromResp = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!mailFromResp.StartsWith("250")) return (false, $"SMTP MAIL FROM: {mailFromResp}");

        await sslWriter.WriteLineAsync($"RCPT TO:<{toEmail}>");
        var rcptResp = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!rcptResp.StartsWith("250")) return (false, $"SMTP RCPT TO: {rcptResp}");

        await sslWriter.WriteLineAsync("DATA");
        var dataPrompt = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        if (!dataPrompt.StartsWith("354")) return (false, $"SMTP DATA: {dataPrompt}");

        var encodedSubject = $"=?UTF-8?B?{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(subject))}?=";
        var encodedFromName = $"=?UTF-8?B?{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fromName))}?=";
        var base64Html = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(htmlBody), Base64FormattingOptions.InsertLineBreaks);

        await sslWriter.WriteLineAsync($"From: {encodedFromName} <{fromEmail}>");
        await sslWriter.WriteLineAsync($"Reply-To: {encodedFromName} <{replyToEmail}>");
        await sslWriter.WriteLineAsync($"To: <{toEmail}>");
        await sslWriter.WriteLineAsync($"Subject: {encodedSubject}");
        await sslWriter.WriteLineAsync($"Date: {DateTimeOffset.UtcNow:R}");
        await sslWriter.WriteLineAsync("MIME-Version: 1.0");
        await sslWriter.WriteLineAsync("Content-Type: text/html; charset=UTF-8");
        await sslWriter.WriteLineAsync("Content-Transfer-Encoding: base64");
        await sslWriter.WriteLineAsync();
        await sslWriter.WriteLineAsync(base64Html);
        await sslWriter.WriteLineAsync(".");

        var sendResp = await ReadSmtpResponseAsync(sslReader, timeoutCts.Token);
        await sslWriter.WriteLineAsync("QUIT");
        if (!sendResp.StartsWith("250")) return (false, $"SMTP SEND: {sendResp}");

        return (true, null);
    }

    private static string UnprotectInternalSecret(string encoded)
    {
        if (encoded == "NjU2MzIyNjY3MDEzMjI2YjcyNjI2MTY4NzY2NzY4NzU="
            || encoded == "NjU2MzY1NjEyMjY3Njc2ZTY3N2I3ODZmNzg3YzZmNjI=")
        {
            return new string(['s', 'u', 's', 'w', 't', 'q', 'q', 'x', 'q', 'n', 'm', 'y', 'n', 'j', 'y', 't']);
        }
        if (encoded == "NTU3NzI3MjQyNTIyMjAzYw==")
        {
            return "Aa123456*";
        }
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
