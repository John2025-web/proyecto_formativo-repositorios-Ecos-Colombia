using ApiPrueba.Models;
using ApiPrueba.Data;
using ApiPrueba.DBcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ApiPrueba.Services;


namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UsuarioService _usuarios;
        private readonly AppDbContext _administradores;
        private readonly RecaptchaVerifier _recaptcha;

        public AuthController(IConfiguration configuration, UsuarioService usuarios, AppDbContext administradores, RecaptchaVerifier recaptcha)
        {
            _configuration = configuration;
            _usuarios = usuarios;
            _administradores = administradores;
            _recaptcha = recaptcha;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (login == null
                || string.IsNullOrWhiteSpace(login.Usuario)
                || string.IsNullOrWhiteSpace(login.contraseña)
                || string.IsNullOrWhiteSpace(login.Rol))
            {
                return BadRequest(new { mensaje = "Usuario, contraseña y rol son obligatorios." });
            }

            if (!await _recaptcha.VerifyAsync(login.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa la verificación reCAPTCHA e inténtalo de nuevo." });
            }

            var rol = login.Rol.Trim();
            var isAdmin = rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            var isUser = rol.Equals("Usuario", StringComparison.OrdinalIgnoreCase);
            var validCredentials = false;

            if (isAdmin)
            {
                validCredentials = login.Usuario == _configuration["Auth:Username"]
                    && login.contraseña == _configuration["Auth:Password"];

                if (!validCredentials)
                {
                    var administrator = await _administradores.ADMINISTRADORES
                        .FirstOrDefaultAsync(x => x.Correo == login.Usuario || x.Nombre == login.Usuario);
                    validCredentials = administrator != null
                        && !string.IsNullOrWhiteSpace(administrator.PasswordHash)
                        && BCrypt.Net.BCrypt.Verify(login.contraseña, administrator.PasswordHash);
                }
            }
            else if (isUser)
            {
                var identifier = login.Usuario.Trim();
                var user = await _usuarios.usuario.FirstOrDefaultAsync(x =>
                    x.Correo == identifier || x.Nombre == identifier);
                validCredentials = user != null
                    && user.Activo
                    && !string.IsNullOrWhiteSpace(user.PasswordHash)
                    && BCrypt.Net.BCrypt.Verify(login.contraseña, user.PasswordHash);
            }

            if (validCredentials)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var expiresAt = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.AddMinutes(25).ToUnixTimeSeconds());

                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, login.Usuario),
                        new Claim(ClaimTypes.Email, login.Usuario),
                        new Claim(ClaimTypes.Role, isAdmin ? "Administrador" : "Usuario"),
                        new Claim("sessionExpiresAt", expiresAt.ToUnixTimeSeconds().ToString())
                    },
                    expires: expiresAt.UtcDateTime,
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                Response.Cookies.Append("access_token", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expiresAt,
                    IsEssential = true
                });
                return Ok(new
                {
                    mensaje = "Inicio de sesión correcto.",
                    token = tokenString,
                    expiraEn = expiresAt
                });
            }

            return Unauthorized();
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
            return NoContent();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { mensaje = "Los datos de registro son obligatorios." });
            }

            if (!await _recaptcha.VerifyAsync(request.CaptchaToken, HttpContext.RequestAborted))
            {
                return BadRequest(new { mensaje = "Completa la verificación reCAPTCHA e inténtalo de nuevo." });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre)
                || string.IsNullOrWhiteSpace(request.Correo)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { mensaje = "Nombre, correo y contraseña son obligatorios." });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });
            }

            if (await _usuarios.usuario.AnyAsync(x => x.Correo == request.Correo))
            {
                return Conflict(new { mensaje = "Ya existe un usuario con ese correo." });
            }

            var user = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Correo = request.Correo.Trim(),
                PaisOrigen = request.PaisOrigen?.Trim() ?? string.Empty,
                IdiomaPreferido = request.IdiomaPreferido?.Trim() ?? "ES",
                TipoUsuario = "Usuario",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _usuarios.usuario.Add(user);
            await _usuarios.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Usuario registrado correctamente."
            });
        }
    }

    public class RegisterRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CaptchaToken { get; set; } = string.Empty;
        public string? PaisOrigen { get; set; }
        public string? IdiomaPreferido { get; set; }
    }
}

