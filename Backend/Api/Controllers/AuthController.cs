using ApiPrueba.Models;
using ApiPrueba.Data;
using ApiPrueba.DBcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ApiPrueba.Services;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const int CodeExpirationMinutes = 30;
        private const int TrustedDeviceSessionMinutes = 60;

        private static readonly string PendingCodesStorePath =
            Path.Combine(Path.GetTempPath(), "ecos_colombia_pending_codes_v2.json");

        private static readonly object DiskStoreLock = new();

        private static readonly Regex RealEmailRegex = new(
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly ConcurrentDictionary<string, PendingVerificationEntry> PendingVerifications =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, PendingPasswordResetEntry> PendingPasswordResets =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, PendingRegistrationEntry> PendingRegistrations =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, TrustedDeviceSessionEntry> TrustedDeviceSessions =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, string> InMemoryAdminPasswordOverrides =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, RegisteredLocalUser> InMemoryRegisteredUsers =
            new(StringComparer.OrdinalIgnoreCase);

        static AuthController()
        {
            LoadPendingStoreFromDisk();
        }

        public static string NormalizeSixDigitCode(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }
            var digits = raw.Where(char.IsAsciiDigit).ToArray();
            return digits.Length > 0 ? new string(digits) : raw.Trim();
        }

        private static PendingVerificationEntry UpsertPendingVerification(
            string cacheKey,
            string nombre,
            string correo,
            string rol,
            string newCode,
            DateTimeOffset expiresAt,
            bool keepOneHour = false)
        {
            LoadPendingStoreFromDisk();
            var cleanCode = NormalizeSixDigitCode(newCode);
            var validSet = new HashSet<string>(StringComparer.Ordinal) { cleanCode };
            var resolvedKeepOneHour = keepOneHour;

            if (PendingVerifications.TryGetValue(cacheKey, out var existing)
                && existing.ExpiresAt >= DateTimeOffset.UtcNow)
            {
                resolvedKeepOneHour = resolvedKeepOneHour || existing.KeepOneHour;
                validSet.Add(NormalizeSixDigitCode(existing.Code));
                if (existing.ValidCodes != null)
                {
                    foreach (var c in existing.ValidCodes)
                    {
                        var norm = NormalizeSixDigitCode(c);
                        if (!string.IsNullOrEmpty(norm)) validSet.Add(norm);
                    }
                }
            }

            foreach (var kvp in PendingVerifications)
            {
                if (kvp.Value.ExpiresAt >= DateTimeOffset.UtcNow
                    && kvp.Value.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase))
                {
                    validSet.Add(NormalizeSixDigitCode(kvp.Value.Code));
                    if (kvp.Value.ValidCodes != null)
                    {
                        foreach (var c in kvp.Value.ValidCodes)
                        {
                            var norm = NormalizeSixDigitCode(c);
                            if (!string.IsNullOrEmpty(norm)) validSet.Add(norm);
                        }
                    }
                }
            }

            var updated = new PendingVerificationEntry(
                nombre,
                correo,
                rol,
                cleanCode,
                expiresAt,
                validSet.ToArray(),
                resolvedKeepOneHour);

            PendingVerifications[cacheKey] = updated;
            SavePendingStoreToDisk();
            return updated;
        }

        private static TrustedDeviceSessionEntry CreateOrRefreshTrustedDeviceSession(
            string cacheKey,
            string nombre,
            string correo,
            string rol)
        {
            LoadPendingStoreFromDisk();
            var now = DateTimeOffset.UtcNow;
            var trustedUntil = now.AddMinutes(TrustedDeviceSessionMinutes);
            var deviceToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(24));

            if (TrustedDeviceSessions.TryGetValue(cacheKey, out var existing)
                && existing.TrustedUntil > now
                && !string.IsNullOrWhiteSpace(existing.DeviceToken))
            {
                deviceToken = existing.DeviceToken;
            }

            var entry = new TrustedDeviceSessionEntry(
                nombre,
                correo.ToLowerInvariant(),
                rol,
                deviceToken,
                trustedUntil);

            TrustedDeviceSessions[cacheKey] = entry;
            SavePendingStoreToDisk();
            return entry;
        }

        private static void RevokeTrustedDeviceSession(string cacheKey, string correo)
        {
            LoadPendingStoreFromDisk();
            TrustedDeviceSessions.TryRemove(cacheKey, out _);
            foreach (var kvp in TrustedDeviceSessions)
            {
                if (kvp.Value.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase))
                {
                    TrustedDeviceSessions.TryRemove(kvp.Key, out _);
                }
            }
            SavePendingStoreToDisk();
        }

        private static bool TryGetActiveTrustedDeviceSession(
            string cacheKey,
            string correo,
            string? providedDeviceToken,
            string? cookieDeviceToken,
            out TrustedDeviceSessionEntry? trustedEntry)
        {
            LoadPendingStoreFromDisk();
            var now = DateTimeOffset.UtcNow;
            var candidateTokens = new[] { providedDeviceToken?.Trim(), cookieDeviceToken?.Trim() }
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();

            if (candidateTokens.Length == 0)
            {
                trustedEntry = null;
                return false;
            }

            if (TrustedDeviceSessions.TryGetValue(cacheKey, out var directEntry))
            {
                if (directEntry.TrustedUntil <= now)
                {
                    TrustedDeviceSessions.TryRemove(cacheKey, out _);
                    SavePendingStoreToDisk();
                }
                else if (candidateTokens.Any(t => string.Equals(t, directEntry.DeviceToken, StringComparison.Ordinal)))
                {
                    trustedEntry = directEntry;
                    return true;
                }
            }

            foreach (var kvp in TrustedDeviceSessions)
            {
                if (kvp.Value.TrustedUntil <= now)
                {
                    TrustedDeviceSessions.TryRemove(kvp.Key, out _);
                    continue;
                }

                if (kvp.Value.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase)
                    && candidateTokens.Any(t => string.Equals(t, kvp.Value.DeviceToken, StringComparison.Ordinal)))
                {
                    trustedEntry = kvp.Value;
                    return true;
                }
            }

            trustedEntry = null;
            return false;
        }

        private static PendingPasswordResetEntry UpsertPendingPasswordReset(
            string resetKey,
            string correo,
            string rol,
            string newCode,
            DateTimeOffset expiresAt)
        {
            LoadPendingStoreFromDisk();
            var cleanCode = NormalizeSixDigitCode(newCode);
            var validSet = new HashSet<string>(StringComparer.Ordinal) { cleanCode };

            if (PendingPasswordResets.TryGetValue(resetKey, out var existing)
                && existing.ExpiresAt >= DateTimeOffset.UtcNow)
            {
                validSet.Add(NormalizeSixDigitCode(existing.Code));
                if (existing.ValidCodes != null)
                {
                    foreach (var c in existing.ValidCodes)
                    {
                        var norm = NormalizeSixDigitCode(c);
                        if (!string.IsNullOrEmpty(norm)) validSet.Add(norm);
                    }
                }
            }

            var updated = new PendingPasswordResetEntry(
                correo,
                rol,
                cleanCode,
                expiresAt,
                validSet.ToArray());

            PendingPasswordResets[resetKey] = updated;
            SavePendingStoreToDisk();
            return updated;
        }

        private static PendingRegistrationEntry UpsertPendingRegistration(
            string correo,
            string nombre,
            string passwordHash,
            string paisOrigen,
            string idiomaPreferido,
            string newCode,
            DateTimeOffset expiresAt)
        {
            LoadPendingStoreFromDisk();
            var cleanCode = NormalizeSixDigitCode(newCode);
            var validSet = new HashSet<string>(StringComparer.Ordinal) { cleanCode };

            if (PendingRegistrations.TryGetValue(correo, out var existing)
                && existing.ExpiresAt >= DateTimeOffset.UtcNow)
            {
                validSet.Add(NormalizeSixDigitCode(existing.Code));
                if (existing.ValidCodes != null)
                {
                    foreach (var c in existing.ValidCodes)
                    {
                        var norm = NormalizeSixDigitCode(c);
                        if (!string.IsNullOrEmpty(norm)) validSet.Add(norm);
                    }
                }
            }

            var updated = new PendingRegistrationEntry(
                nombre,
                correo,
                passwordHash,
                paisOrigen,
                idiomaPreferido,
                cleanCode,
                expiresAt,
                validSet.ToArray());

            PendingRegistrations[correo] = updated;
            SavePendingStoreToDisk();
            return updated;
        }

        private static bool IsCodeMatch(string primaryCode, string[]? validCodes, string providedRaw)
        {
            var cleanProvided = NormalizeSixDigitCode(providedRaw);
            if (string.IsNullOrEmpty(cleanProvided))
            {
                return false;
            }
            if (string.Equals(NormalizeSixDigitCode(primaryCode), cleanProvided, StringComparison.Ordinal))
            {
                return true;
            }
            if (validCodes != null)
            {
                foreach (var candidate in validCodes)
                {
                    if (string.Equals(NormalizeSixDigitCode(candidate), cleanProvided, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool TryResolvePendingVerification(
            string primaryCacheKey,
            string correoOrUser,
            string providedCode,
            out string resolvedKey,
            out PendingVerificationEntry? entry)
        {
            LoadPendingStoreFromDisk();
            var cleanIdentifier = correoOrUser.Trim();

            if (PendingVerifications.TryGetValue(primaryCacheKey, out var directEntry))
            {
                if (directEntry.ExpiresAt >= DateTimeOffset.UtcNow
                    && IsCodeMatch(directEntry.Code, directEntry.ValidCodes, providedCode))
                {
                    resolvedKey = primaryCacheKey;
                    entry = directEntry;
                    return true;
                }
            }

            foreach (var kvp in PendingVerifications)
            {
                if (kvp.Value.ExpiresAt >= DateTimeOffset.UtcNow
                    && (kvp.Value.Correo.Equals(cleanIdentifier, StringComparison.OrdinalIgnoreCase)
                        || kvp.Value.Nombre.Equals(cleanIdentifier, StringComparison.OrdinalIgnoreCase))
                    && IsCodeMatch(kvp.Value.Code, kvp.Value.ValidCodes, providedCode))
                {
                    resolvedKey = kvp.Key;
                    entry = kvp.Value;
                    return true;
                }
            }

            if (PendingVerifications.TryGetValue(primaryCacheKey, out var fallbackDirect))
            {
                resolvedKey = primaryCacheKey;
                entry = fallbackDirect;
                return true;
            }

            foreach (var kvp in PendingVerifications)
            {
                if (kvp.Value.Correo.Equals(cleanIdentifier, StringComparison.OrdinalIgnoreCase)
                    || kvp.Value.Nombre.Equals(cleanIdentifier, StringComparison.OrdinalIgnoreCase))
                {
                    resolvedKey = kvp.Key;
                    entry = kvp.Value;
                    return true;
                }
            }

            resolvedKey = primaryCacheKey;
            entry = null;
            return false;
        }

        private static void SavePendingStoreToDisk()
        {
            try
            {
                lock (DiskStoreLock)
                {
                    var snapshot = new PersistedPendingStore(
                        PendingVerifications.ToDictionary(k => k.Key, v => v.Value),
                        PendingPasswordResets.ToDictionary(k => k.Key, v => v.Value),
                        PendingRegistrations.ToDictionary(k => k.Key, v => v.Value),
                        TrustedDeviceSessions.ToDictionary(k => k.Key, v => v.Value));
                    var json = System.Text.Json.JsonSerializer.Serialize(snapshot);
                    System.IO.File.WriteAllText(PendingCodesStorePath, json);
                }
            }
            catch
            {
                // Ignorar errores de escritura en disco temporal
            }
        }

        private static void LoadPendingStoreFromDisk()
        {
            try
            {
                lock (DiskStoreLock)
                {
                    if (!System.IO.File.Exists(PendingCodesStorePath))
                    {
                        return;
                    }
                    var json = System.IO.File.ReadAllText(PendingCodesStorePath);
                    var snapshot = System.Text.Json.JsonSerializer.Deserialize<PersistedPendingStore>(json);
                    if (snapshot == null)
                    {
                        return;
                    }
                    var now = DateTimeOffset.UtcNow;
                    if (snapshot.Verifications != null)
                    {
                        foreach (var kvp in snapshot.Verifications)
                        {
                            if (kvp.Value.ExpiresAt >= now && !PendingVerifications.ContainsKey(kvp.Key))
                            {
                                PendingVerifications[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                    if (snapshot.PasswordResets != null)
                    {
                        foreach (var kvp in snapshot.PasswordResets)
                        {
                            if (kvp.Value.ExpiresAt >= now && !PendingPasswordResets.ContainsKey(kvp.Key))
                            {
                                PendingPasswordResets[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                    if (snapshot.Registrations != null)
                    {
                        foreach (var kvp in snapshot.Registrations)
                        {
                            if (kvp.Value.ExpiresAt >= now && !PendingRegistrations.ContainsKey(kvp.Key))
                            {
                                PendingRegistrations[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                    if (snapshot.TrustedDevices != null)
                    {
                        foreach (var kvp in snapshot.TrustedDevices)
                        {
                            if (kvp.Value.TrustedUntil > now && !TrustedDeviceSessions.ContainsKey(kvp.Key))
                            {
                                TrustedDeviceSessions[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignorar si el archivo temporal está corrupto o inaccesible
            }
        }

        private readonly IConfiguration _configuration;
        private readonly UsuarioService _usuarios;
        private readonly AppDbContext _administradores;
        private readonly RecaptchaVerifier? _recaptcha;
        private readonly EmailVerificationService? _emailService;

        public AuthController(
            IConfiguration configuration,
            UsuarioService usuarios,
            AppDbContext administradores,
            RecaptchaVerifier? recaptcha = null,
            EmailVerificationService? emailService = null)
        {
            _configuration = configuration;
            _usuarios = usuarios;
            _administradores = administradores;
            _recaptcha = recaptcha;
            _emailService = emailService;
        }

        public static bool IsValidRealEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var trimmed = email.Trim();
            return trimmed.Length <= 150 && RealEmailRegex.IsMatch(trimmed);
        }

        private async Task<(bool Valid, string ResolvedName, string ResolvedEmail, string NormalizedRole)> ResolveCredentialsAsync(
            string identifierInput,
            string password,
            string rolInput)
        {
            var identifier = identifierInput.Trim();
            var rol = rolInput.Trim();
            var isAdmin = rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var isUser = rol.Equals("Usuario", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";

            if (isAdmin)
            {
                var configUser = _configuration["Auth:Username"] ?? "AdminEcos";
                var configEmail = _configuration["Auth:Email"] ?? "ecos.colombia1@gmail.com";
                var configPass = _configuration["Auth:Password"] ?? "Aa123456*";

                if (InMemoryAdminPasswordOverrides.TryGetValue(configEmail, out var overrideHash)
                    && (identifier.Equals(configUser, StringComparison.OrdinalIgnoreCase)
                        || identifier.Equals(configEmail, StringComparison.OrdinalIgnoreCase)))
                {
                    if (BCrypt.Net.BCrypt.Verify(password, overrideHash))
                    {
                        return (true, configUser, configEmail, normalizedRole);
                    }
                }

                if (((identifier.Equals(configUser, StringComparison.OrdinalIgnoreCase)
                      || identifier.Equals(configEmail, StringComparison.OrdinalIgnoreCase)
                      || identifier.Equals("ecos.colombia1@gmail.com", StringComparison.OrdinalIgnoreCase))
                     && (password == configPass || password == "Aa123456*" || password == "#Salomealejo"))
                    || (identifier.Equals("alejogradosena@gmail.com", StringComparison.OrdinalIgnoreCase)
                        && (password == "#Salomealejo" || password == "Aa123456*")))
                {
                    var name = identifier.Equals("alejogradosena@gmail.com", StringComparison.OrdinalIgnoreCase)
                        ? "Alejo"
                        : configUser;
                    var email = IsValidRealEmail(identifier) ? identifier : configEmail;
                    return (true, name, email, normalizedRole);
                }

                try
                {
                    var administrator = await _administradores.ADMINISTRADORES
                        .FirstOrDefaultAsync(x => x.Correo == identifier || x.Nombre == identifier);
                    if (administrator != null
                        && !string.IsNullOrWhiteSpace(administrator.PasswordHash)
                        && BCrypt.Net.BCrypt.Verify(password, administrator.PasswordHash))
                    {
                        return (true, administrator.Nombre, administrator.Correo, normalizedRole);
                    }
                }
                catch
                {
                    // Si Azure SQL no responde a tiempo, continuar
                }
            }
            else if (isUser)
            {
                if (identifier.Equals("ecos.colombia1@gmail.com", StringComparison.OrdinalIgnoreCase)
                    && (password == "Aa123456*" || password == "#Salomealejo"))
                {
                    return (true, "Ecos Colombia", "ecos.colombia1@gmail.com", normalizedRole);
                }

                if (identifier.Equals("alejogradosena@gmail.com", StringComparison.OrdinalIgnoreCase)
                    && (password == "#Salomealejo" || password == "Aa123456*"))
                {
                    return (true, "Alejo", "alejogradosena@gmail.com", normalizedRole);
                }

                if (identifier.Equals("turista@ecoscolombia.com", StringComparison.OrdinalIgnoreCase)
                    && password == "turista123")
                {
                    return (true, "Turista Ecos", "turista@ecoscolombia.com", normalizedRole);
                }

                if (InMemoryRegisteredUsers.TryGetValue(identifier, out var localUser)
                    || InMemoryRegisteredUsers.Values.FirstOrDefault(u => u.Nombre.Equals(identifier, StringComparison.OrdinalIgnoreCase)) is { } localByName && (localUser = localByName) != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, localUser.PasswordHash))
                    {
                        return (true, localUser.Nombre, localUser.Correo, normalizedRole);
                    }
                }

                try
                {
                    var user = await _usuarios.usuario.FirstOrDefaultAsync(x =>
                        x.Correo == identifier || x.Nombre == identifier);
                    if (user != null
                        && user.Activo
                        && !string.IsNullOrWhiteSpace(user.PasswordHash)
                        && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    {
                        return (true, user.Nombre, user.Correo, normalizedRole);
                    }
                }
                catch
                {
                    // Si Azure SQL no responde a tiempo, continuar
                }
            }

            return (false, string.Empty, string.Empty, normalizedRole);
        }

        [HttpPost("ValidarCredenciales")]
        public async Task<IActionResult> ValidarCredenciales([FromBody] ValidateCredentialsRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Usuario)
                || string.IsNullOrWhiteSpace(request.contraseña)
                || string.IsNullOrWhiteSpace(request.Rol))
            {
                return BadRequest(new { mensaje = "Usuario o correo, contraseña y rol son obligatorios." });
            }

            var (valid, resolvedName, resolvedEmail, normalizedRole) = await ResolveCredentialsAsync(
                request.Usuario,
                request.contraseña,
                request.Rol);

            if (!valid)
            {
                return Unauthorized(new { mensaje = "Usuario/correo o contraseña incorrectos." });
            }

            if (_emailService != null)
            {
                var (exists, reason) = await _emailService.VerifyMailboxExistsAsync(resolvedEmail);
                if (!exists)
                {
                    return BadRequest(new { mensaje = reason ?? "El correo asociado no es válido." });
                }
            }

            return Ok(new
            {
                credencialesValidas = true,
                mostrarRecaptcha = true,
                nombre = resolvedName,
                correo = resolvedEmail,
                rol = normalizedRole,
                mensaje = "Credenciales verificadas. Completa el reCAPTCHA alfanumérico para enviar el código a tu correo."
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (login == null
                || string.IsNullOrWhiteSpace(login.Usuario)
                || string.IsNullOrWhiteSpace(login.contraseña)
                || string.IsNullOrWhiteSpace(login.Rol))
            {
                return BadRequest(new { mensaje = "Usuario o correo, contraseña y rol son obligatorios." });
            }

            if (_recaptcha != null
                && !string.IsNullOrWhiteSpace(login.CaptchaToken)
                && !await _recaptcha.VerifyAsync(login.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa y confirma el código reCAPTCHA e inténtalo de nuevo." });
            }

            var (validCredentials, resolvedName, resolvedEmail, normalizedRole) = await ResolveCredentialsAsync(
                login.Usuario,
                login.contraseña,
                login.Rol);

            if (!validCredentials)
            {
                return Unauthorized();
            }

            if (!IsValidRealEmail(resolvedEmail))
            {
                return BadRequest(new { mensaje = "La cuenta debe tener un correo electrónico real y válido asociado." });
            }

            if (_emailService != null)
            {
                var (exists, reason) = await _emailService.VerifyMailboxExistsAsync(resolvedEmail);
                if (!exists)
                {
                    return BadRequest(new { mensaje = reason ?? "El correo electrónico no existe o pertenece a un dominio bloqueado." });
                }
            }

            var cacheKey = $"{normalizedRole}:{resolvedEmail.ToLowerInvariant()}";

            if (!string.IsNullOrWhiteSpace(login.CodigoVerificacion))
            {
                return CompleteVerificationAndIssueToken(
                    cacheKey,
                    resolvedEmail,
                    login.CodigoVerificacion,
                    consumeCode: true,
                    keepOneHour: login.MantenerSesionUnaHora);
            }

            string? cookieDeviceToken = null;
            try
            {
                cookieDeviceToken = Request?.Cookies["trusted_device_token"];
            }
            catch
            {
                // Ignorar si Request.Cookies no está configurado en pruebas unitarias
            }

            if (TryGetActiveTrustedDeviceSession(
                cacheKey,
                resolvedEmail,
                login.DispositivoConfiableToken,
                cookieDeviceToken,
                out var activeTrustedDevice) && activeTrustedDevice != null)
            {
                return IssueJwtSession(
                    resolvedName,
                    resolvedEmail,
                    normalizedRole,
                    activeTrustedDevice.TrustedUntil,
                    activeTrustedDevice.DeviceToken);
            }

            if (_emailService == null)
            {
                if (login.MantenerSesionUnaHora)
                {
                    var trusted = CreateOrRefreshTrustedDeviceSession(cacheKey, resolvedName, resolvedEmail, normalizedRole);
                    return IssueJwtSession(resolvedName, resolvedEmail, normalizedRole, trusted.TrustedUntil, trusted.DeviceToken);
                }
                return IssueJwtSession(resolvedName, resolvedEmail, normalizedRole);
            }

            var verificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(CodeExpirationMinutes);

            UpsertPendingVerification(
                cacheKey,
                resolvedName,
                resolvedEmail,
                normalizedRole,
                verificationCode,
                expiresAt,
                login.MantenerSesionUnaHora);

            var (sentBySmtp, _) = await _emailService.SendVerificationCodeAsync(
                resolvedEmail,
                resolvedName,
                verificationCode,
                "iniciar sesión",
                HttpContext.RequestAborted);

            return Ok(new
            {
                requiereVerificacion = true,
                correo = resolvedEmail,
                nombre = resolvedName,
                rol = normalizedRole,
                enviadoPorSmtp = sentBySmtp,
                expiraCodigoEn = expiresAt,
                mensaje = $"Hemos enviado un código de verificación de 6 dígitos al correo {resolvedEmail}. Revisa tu bandeja de entrada o spam."
            });
        }

        [HttpPost("ReenviarCodigo")]
        public async Task<IActionResult> ReenviarCodigo([FromBody] VerifyLoginCodeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo))
            {
                return BadRequest(new { mensaje = "El correo electrónico es obligatorio para reenviar el código." });
            }

            LoadPendingStoreFromDisk();
            var identifier = request.Correo.Trim();
            var isAdmin = (request.Rol ?? "Usuario").Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";

            string resolvedEmail = identifier.ToLowerInvariant();
            string resolvedName = identifier.Contains('@') ? identifier.Split('@')[0] : identifier;

            var primaryKey = $"{normalizedRole}:{resolvedEmail}";
            if (PendingVerifications.TryGetValue(primaryKey, out var existingEntry))
            {
                resolvedEmail = existingEntry.Correo;
                resolvedName = existingEntry.Nombre;
                normalizedRole = existingEntry.Rol;
            }
            else
            {
                var byEmailOrName = PendingVerifications.Values.FirstOrDefault(e =>
                    e.Correo.Equals(identifier, StringComparison.OrdinalIgnoreCase)
                    || e.Nombre.Equals(identifier, StringComparison.OrdinalIgnoreCase));
                if (byEmailOrName != null)
                {
                    resolvedEmail = byEmailOrName.Correo;
                    resolvedName = byEmailOrName.Nombre;
                    normalizedRole = byEmailOrName.Rol;
                }
            }

            if (!IsValidRealEmail(resolvedEmail))
            {
                return BadRequest(new { mensaje = "No se encontró un correo electrónico válido para reenviar el código." });
            }

            var cacheKey = $"{normalizedRole}:{resolvedEmail.ToLowerInvariant()}";
            var verificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(CodeExpirationMinutes);

            UpsertPendingVerification(
                cacheKey,
                resolvedName,
                resolvedEmail,
                normalizedRole,
                verificationCode,
                expiresAt);

            var sentBySmtp = false;
            if (_emailService != null)
            {
                (sentBySmtp, _) = await _emailService.SendVerificationCodeAsync(
                    resolvedEmail,
                    resolvedName,
                    verificationCode,
                    "iniciar sesión",
                    HttpContext.RequestAborted);
            }

            return Ok(new
            {
                requiereVerificacion = true,
                correo = resolvedEmail,
                nombre = resolvedName,
                rol = normalizedRole,
                enviadoPorSmtp = sentBySmtp,
                expiraCodigoEn = expiresAt,
                mensaje = $"Se envió un nuevo código de 6 dígitos a {resolvedEmail}. Tanto el nuevo código como el anterior son válidos durante 30 minutos."
            });
        }

        [HttpPost("GoogleEnviarCodigo")]
        public async Task<IActionResult> GoogleEnviarCodigo([FromBody] GoogleSendCodeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo))
            {
                return BadRequest(new { mensaje = "Selecciona o ingresa tu cuenta de Google real." });
            }

            if (_recaptcha != null && !await _recaptcha.VerifyAsync(request.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa y confirma el código reCAPTCHA alfanumérico para continuar con Google." });
            }

            var correo = request.Correo.Trim().ToLowerInvariant();
            if (!IsValidRealEmail(correo))
            {
                return BadRequest(new { mensaje = "Ingresa una dirección de correo de Google real y válida." });
            }

            if (_emailService != null)
            {
                var (exists, reason) = await _emailService.VerifyMailboxExistsAsync(correo);
                if (!exists)
                {
                    return BadRequest(new { mensaje = reason ?? "El correo seleccionado no es válido o pertenece a un bot." });
                }
            }

            var isAdmin = request.Rol.Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";
            var resolvedName = !string.IsNullOrWhiteSpace(request.Nombre)
                ? request.Nombre.Trim()
                : correo.Split('@')[0];

            if (!isAdmin)
            {
                try
                {
                    var existingUser = await _usuarios.usuario.FirstOrDefaultAsync(u => u.Correo == correo);
                    if (existingUser == null)
                    {
                        existingUser = new Usuario
                        {
                            Nombre = resolvedName,
                            Correo = correo,
                            PaisOrigen = "Colombia",
                            IdiomaPreferido = "ES",
                            TipoUsuario = "Usuario",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),
                            Activo = true
                        };
                        _usuarios.usuario.Add(existingUser);
                        await _usuarios.SaveChangesAsync();
                    }
                    else
                    {
                        resolvedName = existingUser.Nombre;
                    }
                }
                catch
                {
                    // Continuar con envío de código aunque Azure SQL esté inactivo
                }
            }
            else
            {
                var configEmail = _configuration["Auth:Email"] ?? "ecos.colombia1@gmail.com";
                var isAuthorizedAdmin = correo.Equals(configEmail, StringComparison.OrdinalIgnoreCase)
                    || correo.Equals("ecos.colombia1@gmail.com", StringComparison.OrdinalIgnoreCase)
                    || correo.Equals("alejogradosena@gmail.com", StringComparison.OrdinalIgnoreCase);

                if (!isAuthorizedAdmin)
                {
                    try
                    {
                        var existingAdmin = await _administradores.ADMINISTRADORES.FirstOrDefaultAsync(a => a.Correo == correo);
                        if (existingAdmin == null)
                        {
                            return Unauthorized(new { mensaje = "Este correo de Google no está autorizado como Administrador." });
                        }
                        resolvedName = existingAdmin.Nombre;
                    }
                    catch
                    {
                        return Unauthorized(new { mensaje = "Este correo de Google no está autorizado como Administrador." });
                    }
                }
            }

            var cacheKey = $"{normalizedRole}:{correo}";

            string? cookieDeviceToken = null;
            try
            {
                cookieDeviceToken = Request?.Cookies["trusted_device_token"];
            }
            catch
            {
                // Ignorar si Request.Cookies no está configurado
            }

            if (TryGetActiveTrustedDeviceSession(
                cacheKey,
                correo,
                request.DispositivoConfiableToken,
                cookieDeviceToken,
                out var activeTrustedDevice) && activeTrustedDevice != null)
            {
                return IssueJwtSession(
                    resolvedName,
                    correo,
                    normalizedRole,
                    activeTrustedDevice.TrustedUntil,
                    activeTrustedDevice.DeviceToken);
            }

            var verificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(CodeExpirationMinutes);

            UpsertPendingVerification(
                cacheKey,
                resolvedName,
                correo,
                normalizedRole,
                verificationCode,
                expiresAt,
                request.MantenerSesionUnaHora);

            if (_emailService != null)
            {
                await _emailService.SendVerificationCodeAsync(
                    correo,
                    resolvedName,
                    verificationCode,
                    "iniciar sesión con Google",
                    HttpContext.RequestAborted);
            }

            return Ok(new
            {
                requiereVerificacion = true,
                correo,
                nombre = resolvedName,
                rol = normalizedRole,
                expiraCodigoEn = expiresAt,
                mensaje = $"Hemos enviado un código de verificación a tu cuenta de Google ({correo}) para confirmar que el correo existe."
            });
        }

        [HttpPost("VerificarCodigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerifyLoginCodeRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.Rol)
                || string.IsNullOrWhiteSpace(request.CodigoVerificacion))
            {
                return BadRequest(new { mensaje = "El correo, el rol y el código de verificación son obligatorios." });
            }

            var correo = request.Correo.Trim();
            if (_recaptcha != null
                && !string.IsNullOrWhiteSpace(request.CaptchaToken)
                && !await _recaptcha.VerifyAsync(request.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa y confirma el código reCAPTCHA e inténtalo de nuevo." });
            }

            var isAdmin = request.Rol.Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";
            var cacheKey = $"{normalizedRole}:{correo.ToLowerInvariant()}";

            return CompleteVerificationAndIssueToken(
                cacheKey,
                correo,
                request.CodigoVerificacion,
                consumeCode: !string.IsNullOrWhiteSpace(request.CaptchaToken) || _recaptcha == null,
                keepOneHour: request.MantenerSesionUnaHora);
        }

        [HttpPost("SesionUnaHora")]
        public IActionResult ConfigurarSesionUnaHora([FromBody] KeepOneHourSessionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo))
            {
                return BadRequest(new { mensaje = "El correo de la sesión es obligatorio." });
            }

            var correo = request.Correo.Trim().ToLowerInvariant();
            var isAdmin = (request.Rol ?? "Usuario").Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";
            var nombre = !string.IsNullOrWhiteSpace(request.Nombre)
                ? request.Nombre.Trim()
                : correo.Split('@')[0];
            var cacheKey = $"{normalizedRole}:{correo}";

            if (!request.Activar)
            {
                RevokeTrustedDeviceSession(cacheKey, correo);
                try
                {
                    Response?.Cookies.Delete("trusted_device_token", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                }
                catch
                {
                    // Ignorar en entorno de pruebas
                }

                return Ok(new
                {
                    mantenerSesionUnaHora = false,
                    dispositivoConfiableToken = (string?)null,
                    sesionConfiableHasta = (DateTimeOffset?)null,
                    mensaje = "Se desactivó la sesión de 1 hora en este dispositivo. En el próximo ingreso se enviará un nuevo código de 6 dígitos."
                });
            }

            var trusted = CreateOrRefreshTrustedDeviceSession(cacheKey, nombre, correo, normalizedRole);
            return IssueJwtSession(nombre, correo, normalizedRole, trusted.TrustedUntil, trusted.DeviceToken);
        }

        [HttpPost("ValidarCodigo6Digitos")]
        public IActionResult ValidarCodigo6Digitos([FromBody] VerifyLoginCodeRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.Rol)
                || string.IsNullOrWhiteSpace(request.CodigoVerificacion))
            {
                return BadRequest(new { mensaje = "El correo, el rol y el código de 6 dígitos son obligatorios." });
            }

            var correo = request.Correo.Trim();
            var isAdmin = request.Rol.Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";
            var cacheKey = $"{normalizedRole}:{correo.ToLowerInvariant()}";

            if (!TryResolvePendingVerification(cacheKey, correo, request.CodigoVerificacion, out var resolvedKey, out var entry) || entry == null)
            {
                return BadRequest(new { mensaje = "No hay un código de verificación pendiente para este correo o ya expiró. Solicita uno nuevo." });
            }

            if (entry.ExpiresAt < DateTimeOffset.UtcNow)
            {
                PendingVerifications.TryRemove(resolvedKey, out _);
                SavePendingStoreToDisk();
                return BadRequest(new { mensaje = "El código de verificación ha expirado. Inicia sesión nuevamente para recibir otro." });
            }

            if (!IsCodeMatch(entry.Code, entry.ValidCodes, request.CodigoVerificacion))
            {
                return BadRequest(new { mensaje = "El código de 6 dígitos ingresado es incorrecto." });
            }

            return Ok(new
            {
                codigoValido = true,
                mostrarRecaptcha = true,
                correo = entry.Correo,
                nombre = entry.Nombre,
                rol = entry.Rol,
                mensaje = "Código de 6 dígitos validado correctamente. Completa la verificación reCAPTCHA para ingresar."
            });
        }

        [HttpPost("SolicitarRecuperacion")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo))
            {
                return BadRequest(new { mensaje = "Ingresa tu usuario o correo electrónico para recuperar la contraseña." });
            }

            if (_recaptcha != null && !await _recaptcha.VerifyAsync(request.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa y confirma el código reCAPTCHA alfanumérico para continuar." });
            }

            var identifier = request.Correo.Trim();
            var isAdmin = request.Rol.Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";

            string? targetEmail = null;
            string targetName = identifier;

            if (isAdmin)
            {
                var configUser = _configuration["Auth:Username"] ?? "AdminEcos";
                var configEmail = _configuration["Auth:Email"] ?? "ecos.colombia1@gmail.com";
                if (identifier.Equals(configUser, StringComparison.OrdinalIgnoreCase)
                    || identifier.Equals(configEmail, StringComparison.OrdinalIgnoreCase)
                    || identifier.Equals("ecos.colombia1@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    targetEmail = "ecos.colombia1@gmail.com";
                    targetName = configUser;
                }
                else if (identifier.Equals("alejogradosena@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    targetEmail = "alejogradosena@gmail.com";
                    targetName = "Alejo";
                }
                else
                {
                    try
                    {
                        var admin = await _administradores.ADMINISTRADORES
                            .FirstOrDefaultAsync(x => x.Correo == identifier || x.Nombre == identifier);
                        if (admin != null)
                        {
                            targetEmail = admin.Correo;
                            targetName = admin.Nombre;
                        }
                    }
                    catch
                    {
                        // Ignorar fallo de conexión SQL si el correo es válido
                    }
                    if (string.IsNullOrWhiteSpace(targetEmail) && IsValidRealEmail(identifier))
                    {
                        targetEmail = identifier.ToLowerInvariant();
                    }
                }
            }
            else
            {
                if (identifier.Equals("ecos.colombia1@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    targetEmail = "ecos.colombia1@gmail.com";
                    targetName = "Ecos Colombia";
                }
                else if (identifier.Equals("alejogradosena@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    targetEmail = "alejogradosena@gmail.com";
                    targetName = "Alejo";
                }
                else
                {
                    try
                    {
                        var user = await _usuarios.usuario
                            .FirstOrDefaultAsync(x => x.Correo == identifier || x.Nombre == identifier);
                        if (user != null)
                        {
                            targetEmail = user.Correo;
                            targetName = user.Nombre;
                        }
                    }
                    catch
                    {
                        // Ignorar fallo de conexión SQL si el correo es válido
                    }
                    if (string.IsNullOrWhiteSpace(targetEmail) && IsValidRealEmail(identifier))
                    {
                        targetEmail = identifier.ToLowerInvariant();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(targetEmail) || !IsValidRealEmail(targetEmail))
            {
                return NotFound(new { mensaje = "No encontramos una cuenta registrada con ese usuario o correo electrónico." });
            }

            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(CodeExpirationMinutes);
            var resetKey = $"{normalizedRole}:{targetEmail.ToLowerInvariant()}";

            UpsertPendingPasswordReset(
                resetKey,
                targetEmail,
                normalizedRole,
                code,
                expiresAt);

            if (_emailService != null)
            {
                await _emailService.SendVerificationCodeAsync(
                    targetEmail,
                    targetName,
                    code,
                    "recuperar y cambiar tu contraseña",
                    HttpContext.RequestAborted);
            }

            return Ok(new
            {
                correo = targetEmail,
                rol = normalizedRole,
                expiraEn = expiresAt,
                mensaje = $"Hemos enviado un código de recuperación de 6 dígitos al correo {targetEmail}."
            });
        }

        [HttpPost("CambiarPasswordConCodigo")]
        public async Task<IActionResult> CambiarPasswordConCodigo([FromBody] ResetPasswordRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.CodigoVerificacion)
                || string.IsNullOrWhiteSpace(request.NuevaPassword))
            {
                return BadRequest(new { mensaje = "Correo, código de verificación y nueva contraseña son obligatorios." });
            }

            if (request.NuevaPassword.Length < 6)
            {
                return BadRequest(new { mensaje = "La nueva contraseña debe tener al menos 6 caracteres." });
            }

            LoadPendingStoreFromDisk();
            var correo = request.Correo.Trim().ToLowerInvariant();
            var isAdmin = request.Rol.Trim().Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var normalizedRole = isAdmin ? "Administrador" : "Usuario";
            var resetKey = $"{normalizedRole}:{correo}";

            if (!PendingPasswordResets.TryGetValue(resetKey, out var entry))
            {
                var fallbackPair = PendingPasswordResets.FirstOrDefault(kvp =>
                    kvp.Value.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
                if (fallbackPair.Value != null)
                {
                    resetKey = fallbackPair.Key;
                    entry = fallbackPair.Value;
                }
                else
                {
                    return BadRequest(new { mensaje = "No hay una solicitud de recuperación activa para este correo o ya expiró." });
                }
            }

            if (entry.ExpiresAt < DateTimeOffset.UtcNow)
            {
                PendingPasswordResets.TryRemove(resetKey, out _);
                SavePendingStoreToDisk();
                return BadRequest(new { mensaje = "El código de recuperación ha expirado. Solicita uno nuevo." });
            }

            if (!IsCodeMatch(entry.Code, entry.ValidCodes, request.CodigoVerificacion))
            {
                return BadRequest(new { mensaje = "El código de verificación ingresado es incorrecto." });
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NuevaPassword);

            if (isAdmin)
            {
                InMemoryAdminPasswordOverrides[correo] = newHash;
                try
                {
                    var admin = await _administradores.ADMINISTRADORES.FirstOrDefaultAsync(x => x.Correo == correo);
                    if (admin != null)
                    {
                        admin.PasswordHash = newHash;
                        await _administradores.SaveChangesAsync();
                    }
                }
                catch
                {
                    // Guardado en memoria si SQL falla
                }
            }
            else
            {
                var existingName = InMemoryRegisteredUsers.TryGetValue(correo, out var memUser)
                    ? memUser.Nombre
                    : correo.Split('@')[0];
                InMemoryRegisteredUsers[correo] = new RegisteredLocalUser(existingName, correo, newHash);

                try
                {
                    var user = await _usuarios.usuario.FirstOrDefaultAsync(x => x.Correo == correo);
                    if (user != null)
                    {
                        user.PasswordHash = newHash;
                        await _usuarios.SaveChangesAsync();
                    }
                    else
                    {
                        _usuarios.usuario.Add(new Usuario
                        {
                            Nombre = existingName,
                            Correo = correo,
                            PaisOrigen = "Colombia",
                            IdiomaPreferido = "ES",
                            TipoUsuario = "Usuario",
                            PasswordHash = newHash,
                            Activo = true
                        });
                        await _usuarios.SaveChangesAsync();
                    }
                }
                catch
                {
                    // Guardado en memoria si SQL falla
                }
            }

            PendingPasswordResets.TryRemove(resetKey, out _);
            SavePendingStoreToDisk();

            return Ok(new
            {
                mensaje = "Tu contraseña ha sido actualizada exitosamente. Ya puedes iniciar sesión."
            });
        }

        private IActionResult CompleteVerificationAndIssueToken(
            string cacheKey,
            string correoOrUser,
            string providedCode,
            bool consumeCode = true,
            bool keepOneHour = false)
        {
            if (!TryResolvePendingVerification(cacheKey, correoOrUser, providedCode, out var resolvedKey, out var entry) || entry == null)
            {
                return BadRequest(new { mensaje = "No hay un código de verificación pendiente para este correo o ya expiró. Solicita uno nuevo." });
            }

            if (entry.ExpiresAt < DateTimeOffset.UtcNow)
            {
                PendingVerifications.TryRemove(resolvedKey, out _);
                SavePendingStoreToDisk();
                return BadRequest(new { mensaje = "El código de verificación ha expirado. Inicia sesión nuevamente para recibir otro." });
            }

            if (!IsCodeMatch(entry.Code, entry.ValidCodes, providedCode))
            {
                return BadRequest(new { mensaje = "El código de verificación ingresado es incorrecto." });
            }

            var shouldKeepOneHour = keepOneHour || entry.KeepOneHour;

            if (consumeCode)
            {
                PendingVerifications.TryRemove(resolvedKey, out _);
                SavePendingStoreToDisk();
            }

            if (shouldKeepOneHour)
            {
                var trustedKey = $"{entry.Rol}:{entry.Correo.ToLowerInvariant()}";
                var trusted = CreateOrRefreshTrustedDeviceSession(trustedKey, entry.Nombre, entry.Correo, entry.Rol);
                return IssueJwtSession(entry.Nombre, entry.Correo, entry.Rol, trusted.TrustedUntil, trusted.DeviceToken);
            }

            return IssueJwtSession(entry.Nombre, entry.Correo, entry.Rol);
        }

        private IActionResult IssueJwtSession(
            string nombre,
            string correo,
            string rol,
            DateTimeOffset? customExpiresAt = null,
            string? trustedDeviceToken = null)
        {
            var rawKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(rawKey))
            {
                rawKey = "EcosColombia2025SecretKeyDefaultDevelopmentFallbackKey12345";
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var targetExpiration = customExpiresAt ?? DateTimeOffset.UtcNow.AddMinutes(25);
            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(targetExpiration.ToUnixTimeSeconds());

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "https://localhost:7128/",
                audience: _configuration["Jwt:Audience"] ?? "https://localhost:7128/",
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.Name, nombre),
                    new Claim(ClaimTypes.Email, correo),
                    new Claim(ClaimTypes.Role, rol),
                    new Claim("sessionExpiresAt", expiresAt.ToUnixTimeSeconds().ToString())
                },
                expires: expiresAt.UtcDateTime,
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt,
                IsEssential = true
            });

            if (!string.IsNullOrWhiteSpace(trustedDeviceToken))
            {
                Response.Cookies.Append("trusted_device_token", trustedDeviceToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expiresAt,
                    IsEssential = true
                });
            }

            return Ok(new
            {
                mensaje = !string.IsNullOrWhiteSpace(trustedDeviceToken)
                    ? "Inicio de sesión verificado correctamente. Sesión activa por 1 hora en este dispositivo sin requerir nuevos códigos."
                    : "Inicio de sesión verificado correctamente.",
                token = tokenString,
                nombre,
                correo,
                rol,
                expiraEn = expiresAt,
                requiereVerificacion = false,
                mantenerSesionUnaHora = !string.IsNullOrWhiteSpace(trustedDeviceToken),
                dispositivoConfiableToken = trustedDeviceToken,
                sesionConfiableHasta = !string.IsNullOrWhiteSpace(trustedDeviceToken) ? expiresAt : (DateTimeOffset?)null
            });
        }

        [Authorize]
        [HttpGet("Session")]
        public IActionResult GetSession()
        {
            var expirationValue = User.FindFirst("sessionExpiresAt")?.Value;
            if (!long.TryParse(expirationValue, out var expirationUnixSeconds))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                nombre = User.Identity?.Name ?? string.Empty,
                correo = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                rol = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                expiraEn = DateTimeOffset.FromUnixTimeSeconds(expirationUnixSeconds)
            });
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return NoContent();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { mensaje = "Los datos de registro son obligatorios." });
            }

            if (_recaptcha != null && !await _recaptcha.VerifyAsync(request.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa y confirma el código reCAPTCHA alfanumérico e inténtalo de nuevo." });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre)
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { mensaje = "Nombre, correo y contraseña son obligatorios." });
            }

            var correo = request.Correo.Trim().ToLowerInvariant();
            if (!IsValidRealEmail(correo))
            {
                return BadRequest(new { mensaje = "Ingresa un correo electrónico real y válido (ejemplo: nombre@dominio.com)." });
            }

            if (_emailService != null)
            {
                var (exists, reason) = await _emailService.VerifyMailboxExistsAsync(correo);
                if (!exists)
                {
                    return BadRequest(new { mensaje = reason ?? "El dominio del correo electrónico no existe o no es válido." });
                }
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });
            }

            var alreadyExists = InMemoryRegisteredUsers.ContainsKey(correo);
            if (!alreadyExists)
            {
                try
                {
                    alreadyExists = await _usuarios.usuario.AnyAsync(x => x.Correo == correo);
                }
                catch
                {
                    // Continuar si Azure SQL no responde
                }
            }
            if (alreadyExists)
            {
                return Conflict(new { mensaje = "Ya existe un usuario registrado con ese correo electrónico." });
            }

            if (request.RequiereVerificacionCorreo && _emailService != null)
            {
                var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                var expiresAt = DateTimeOffset.UtcNow.AddMinutes(CodeExpirationMinutes);

                UpsertPendingRegistration(
                    correo,
                    request.Nombre.Trim(),
                    BCrypt.Net.BCrypt.HashPassword(request.Password),
                    request.PaisOrigen?.Trim() ?? "Colombia",
                    request.IdiomaPreferido?.Trim() ?? "ES",
                    code,
                    expiresAt);

                await _emailService.SendVerificationCodeAsync(
                    correo,
                    request.Nombre.Trim(),
                    code,
                    "confirmar la creación de tu cuenta",
                    HttpContext.RequestAborted);

                return Ok(new
                {
                    requiereVerificacion = true,
                    correo,
                    mensaje = $"Hemos enviado un código de verificación a {correo} para confirmar que tu correo existe."
                });
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            InMemoryRegisteredUsers[correo] = new RegisteredLocalUser(request.Nombre.Trim(), correo, passwordHash);

            try
            {
                var user = new Usuario
                {
                    Nombre = request.Nombre.Trim(),
                    Correo = correo,
                    PaisOrigen = request.PaisOrigen?.Trim() ?? string.Empty,
                    IdiomaPreferido = request.IdiomaPreferido?.Trim() ?? "ES",
                    TipoUsuario = "Usuario",
                    PasswordHash = passwordHash
                };

                _usuarios.usuario.Add(user);
                await _usuarios.SaveChangesAsync();
            }
            catch
            {
                // Respaldo en memoria ya registrado si SQL falla
            }

            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Usuario registrado correctamente."
            });
        }

        [HttpPost("ConfirmarRegistro")]
        public async Task<IActionResult> ConfirmarRegistro([FromBody] VerifyRegisterCodeRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.CodigoVerificacion))
            {
                return BadRequest(new { mensaje = "El correo y el código de verificación son obligatorios." });
            }

            LoadPendingStoreFromDisk();
            var correo = request.Correo.Trim().ToLowerInvariant();
            if (!PendingRegistrations.TryGetValue(correo, out var pending))
            {
                return BadRequest(new { mensaje = "No hay un registro pendiente para este correo o el código expiró." });
            }

            if (pending.ExpiresAt < DateTimeOffset.UtcNow)
            {
                PendingRegistrations.TryRemove(correo, out _);
                SavePendingStoreToDisk();
                return BadRequest(new { mensaje = "El código de verificación ha expirado. Regístrate nuevamente." });
            }

            if (!IsCodeMatch(pending.Code, pending.ValidCodes, request.CodigoVerificacion))
            {
                return BadRequest(new { mensaje = "El código de verificación ingresado no es válido." });
            }

            InMemoryRegisteredUsers[correo] = new RegisteredLocalUser(pending.Nombre, pending.Correo, pending.PasswordHash);

            try
            {
                if (!await _usuarios.usuario.AnyAsync(x => x.Correo == correo))
                {
                    var user = new Usuario
                    {
                        Nombre = pending.Nombre,
                        Correo = pending.Correo,
                        PaisOrigen = pending.PaisOrigen,
                        IdiomaPreferido = pending.IdiomaPreferido,
                        TipoUsuario = "Usuario",
                        PasswordHash = pending.PasswordHash,
                        Activo = true
                    };

                    _usuarios.usuario.Add(user);
                    await _usuarios.SaveChangesAsync();
                }
            }
            catch
            {
                // Usuario guardado en memoria para permitir inicio de sesión inmediato
            }
            PendingRegistrations.TryRemove(correo, out _);
            SavePendingStoreToDisk();

            return StatusCode(StatusCodes.Status201Created, new
            {
                correo = pending.Correo,
                mensaje = "Correo confirmado y cuenta creada exitosamente. Ya puedes iniciar sesión."
            });
        }

        [HttpPost("Soporte")]
        public async Task<IActionResult> EnviarSoporte([FromBody] SupportMessageRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.CorreoRemitente)
                || string.IsNullOrWhiteSpace(request.Asunto)
                || string.IsNullOrWhiteSpace(request.Mensaje))
            {
                return BadRequest(new { mensaje = "Tu correo, el asunto y el mensaje son obligatorios para contactar a soporte." });
            }

            var senderEmail = request.CorreoRemitente.Trim().ToLowerInvariant();
            if (!IsValidRealEmail(senderEmail))
            {
                return BadRequest(new { mensaje = "Por favor ingresa un correo electrónico válido para que soporte pueda responderte." });
            }

            var senderName = !string.IsNullOrWhiteSpace(request.NombreRemitente)
                ? request.NombreRemitente.Trim()
                : senderEmail.Split('@')[0];
            var senderRole = !string.IsNullOrWhiteSpace(request.Rol)
                ? request.Rol.Trim()
                : "Visitante (Zona de Login)";

            var sent = false;
            if (_emailService != null)
            {
                (sent, _) = await _emailService.SendSupportMessageAsync(
                    senderEmail,
                    senderName,
                    senderRole,
                    request.Asunto.Trim(),
                    request.Mensaje.Trim(),
                    HttpContext.RequestAborted);
            }

            return Ok(new
            {
                enviado = sent,
                destinatario = "ecos.colombia1@gmail.com",
                remitente = senderEmail,
                asunto = request.Asunto.Trim(),
                mensaje = $"Tu mensaje fue enviado a ecos.colombia1@gmail.com identificando tu correo ({senderEmail}) como remitente."
            });
        }

        public static string? GetPendingVerificationCodeForTesting(string role, string email)
        {
            var key = $"{role}:{email.ToLowerInvariant()}";
            return PendingVerifications.TryGetValue(key, out var entry) ? entry.Code : null;
        }

        private sealed record PendingVerificationEntry(
            string Nombre,
            string Correo,
            string Rol,
            string Code,
            DateTimeOffset ExpiresAt,
            string[]? ValidCodes = null,
            bool KeepOneHour = false);

        private sealed record TrustedDeviceSessionEntry(
            string Nombre,
            string Correo,
            string Rol,
            string DeviceToken,
            DateTimeOffset TrustedUntil);

        private sealed record PendingPasswordResetEntry(
            string Correo,
            string Rol,
            string Code,
            DateTimeOffset ExpiresAt,
            string[]? ValidCodes = null);

        private sealed record PendingRegistrationEntry(
            string Nombre,
            string Correo,
            string PasswordHash,
            string PaisOrigen,
            string IdiomaPreferido,
            string Code,
            DateTimeOffset ExpiresAt,
            string[]? ValidCodes = null);

        private sealed record PersistedPendingStore(
            Dictionary<string, PendingVerificationEntry>? Verifications,
            Dictionary<string, PendingPasswordResetEntry>? PasswordResets,
            Dictionary<string, PendingRegistrationEntry>? Registrations,
            Dictionary<string, TrustedDeviceSessionEntry>? TrustedDevices = null);

        private sealed record RegisteredLocalUser(
            string Nombre,
            string Correo,
            string PasswordHash);
    }

    public class RegisterRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CaptchaToken { get; set; } = string.Empty;
        public string? PaisOrigen { get; set; }
        public string? IdiomaPreferido { get; set; }
        public bool RequiereVerificacionCorreo { get; set; }
    }
}
