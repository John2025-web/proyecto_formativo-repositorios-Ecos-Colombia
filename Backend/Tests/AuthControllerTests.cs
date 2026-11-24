using ApiPrueba.Controllers;
using ApiPrueba.Data;
using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using ApiPrueba.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ApiPrueba.Tests;

public class AuthControllerTests
{
    private readonly IConfiguration _config;

    public AuthControllerTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Issuer", "EcosColombiaApi" },
            { "Jwt:Audience", "EcosColombiaApp" },
            { "Auth:Username", "admin" },
            { "Auth:Email", "ecos.colombia1@gmail.com" },
            { "Auth:Password", "admin123" },
            { "Email:SmtpHost", "smtp.gmail.com" },
            { "Email:SmtpPort", "587" },
            { "Email:SmtpUser", "ecos.colombia1@gmail.com" },
            { "Email:SmtpPass", "" },
            { "Email:SmtpSecretToken", "NzM3NDZjNjU3NTc1NzE2NDc0NjY2Njc5NmU3NzYydw==" },
            { "Email:FromEmail", "ecos.colombia1@gmail.com" }
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "MiClaveSuperSecretaDePruebaParaEcosColombia2026*");
    }

    private static (UsuarioService usuarioDb, AppDbContext adminDb) CreateInMemoryDatabases()
    {
        var dbName = Guid.NewGuid().ToString();

        var usuarioOptions = new DbContextOptionsBuilder<UsuarioService>()
            .UseInMemoryDatabase(dbName + "_user")
            .Options;
        var usuarioDb = new UsuarioService(usuarioOptions);

        var adminOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName + "_admin")
            .Options;
        var adminDb = new AppDbContext(adminOptions);

        return (usuarioDb, adminDb);
    }

    private AuthController CreateController(
        UsuarioService userDb,
        AppDbContext adminDb,
        RecaptchaVerifier? recaptcha = null,
        EmailVerificationService? emailService = null)
    {
        var controller = new AuthController(_config, userDb, adminDb, recaptcha, emailService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        return controller;
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenPayloadIsNull()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var result = await controller.Login(null!);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Theory]
    [InlineData("", "password", "Administrador")]
    [InlineData("admin", "", "Administrador")]
    [InlineData("admin", "password", "")]
    public async Task Login_ReturnsBadRequest_WhenFieldsAreEmpty(string usuario, string password, string rol)
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var login = new Login { Usuario = usuario, contraseña = password, Rol = rol };
        var result = await controller.Login(login);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOkWithToken_WhenAdminDevCredentialsProvided()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var login = new Login { Usuario = "admin", contraseña = "admin123", Rol = "Administrador" };
        var result = await controller.Login(login);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsOkWithToken_WhenTuristaDevCredentialsProvided()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var login = new Login { Usuario = "turista@ecoscolombia.com", contraseña = "turista123", Rol = "Usuario" };
        var result = await controller.Login(login);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_SendsVerificationCodeAndVerifies_WhenEmailServiceAndAlphanumericCaptchaEnabled()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        using var httpClient = new HttpClient();
        var recaptcha = new RecaptchaVerifier(httpClient, _config, NullLogger<RecaptchaVerifier>.Instance);
        var emailService = new EmailVerificationService(_config, NullLogger<EmailVerificationService>.Instance);
        var controller = CreateController(userDb, adminDb, recaptcha, emailService);

        var login = new Login
        {
            Usuario = "turista@ecoscolombia.com",
            contraseña = "turista123",
            Rol = "Usuario",
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:A7b9K2"
        };

        var step1Result = await controller.Login(login);
        var step1Ok = Assert.IsType<OkObjectResult>(step1Result);
        var step1Json = System.Text.Json.JsonSerializer.Serialize(step1Ok.Value);
        using var doc = System.Text.Json.JsonDocument.Parse(step1Json);
        Assert.True(doc.RootElement.GetProperty("requiereVerificacion").GetBoolean());
        Assert.False(doc.RootElement.TryGetProperty("codigoSimulado", out _));
        var generatedCode = AuthController.GetPendingVerificationCodeForTesting("Usuario", "turista@ecoscolombia.com");
        Assert.False(string.IsNullOrWhiteSpace(generatedCode));

        var verifyResult = await controller.VerificarCodigo(new VerifyLoginCodeRequest
        {
            Correo = "turista@ecoscolombia.com",
            Rol = "Usuario",
            CodigoVerificacion = generatedCode!,
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:A7b9K2"
        });

        var verifyOk = Assert.IsType<OkObjectResult>(verifyResult);
        Assert.Equal(200, verifyOk.StatusCode);
    }

    [Fact]
    public async Task Login_WorksWithRealConfiguredCredentials_AlejoGradoSena()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        using var httpClient = new HttpClient();
        var recaptcha = new RecaptchaVerifier(httpClient, _config, NullLogger<RecaptchaVerifier>.Instance);
        var emailService = new EmailVerificationService(_config, NullLogger<EmailVerificationService>.Instance);
        var controller = CreateController(userDb, adminDb, recaptcha, emailService);

        var preCheck = await controller.ValidarCredenciales(new ValidateCredentialsRequest
        {
            Usuario = "alejogradosena@gmail.com",
            contraseña = "#Salomealejo",
            Rol = "Usuario"
        });
        Assert.IsType<OkObjectResult>(preCheck);

        var login = new Login
        {
            Usuario = "alejogradosena@gmail.com",
            contraseña = "#Salomealejo",
            Rol = "Usuario",
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:X8k2M9"
        };

        var step1Result = await controller.Login(login);
        var step1Ok = Assert.IsType<OkObjectResult>(step1Result);
        var step1Json = System.Text.Json.JsonSerializer.Serialize(step1Ok.Value);
        using var doc = System.Text.Json.JsonDocument.Parse(step1Json);
        Assert.True(doc.RootElement.GetProperty("requiereVerificacion").GetBoolean());
        Assert.Equal("alejogradosena@gmail.com", doc.RootElement.GetProperty("correo").GetString());
        Assert.False(doc.RootElement.TryGetProperty("codigoSimulado", out _));
        var generatedCode = AuthController.GetPendingVerificationCodeForTesting("Usuario", "alejogradosena@gmail.com");
        Assert.False(string.IsNullOrWhiteSpace(generatedCode));

        var check6Digit = controller.ValidarCodigo6Digitos(new VerifyLoginCodeRequest
        {
            Correo = "alejogradosena@gmail.com",
            Rol = "Usuario",
            CodigoVerificacion = generatedCode!
        });
        Assert.IsType<OkObjectResult>(check6Digit);

        var verifyResult = await controller.VerificarCodigo(new VerifyLoginCodeRequest
        {
            Correo = "alejogradosena@gmail.com",
            Rol = "Usuario",
            CodigoVerificacion = generatedCode!,
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:X8k2M9"
        });

        var verifyOk = Assert.IsType<OkObjectResult>(verifyResult);
        Assert.Equal(200, verifyOk.StatusCode);
    }

    [Fact]
    public async Task Login_And_PasswordRecovery_WorksWithSenderAccount_EcosColombia1()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        using var httpClient = new HttpClient();
        var recaptcha = new RecaptchaVerifier(httpClient, _config, NullLogger<RecaptchaVerifier>.Instance);
        var emailService = new EmailVerificationService(_config, NullLogger<EmailVerificationService>.Instance);
        var controller = CreateController(userDb, adminDb, recaptcha, emailService);

        var login = new Login
        {
            Usuario = "ecos.colombia1@gmail.com",
            contraseña = "Aa123456*",
            Rol = "Administrador",
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:E7c9O1"
        };

        var loginStep = await controller.Login(login);
        var loginOk = Assert.IsType<OkObjectResult>(loginStep);
        var loginJson = System.Text.Json.JsonSerializer.Serialize(loginOk.Value);
        using var doc = System.Text.Json.JsonDocument.Parse(loginJson);
        Assert.True(doc.RootElement.GetProperty("requiereVerificacion").GetBoolean());
        Assert.True(doc.RootElement.GetProperty("enviadoPorSmtp").GetBoolean());
        Assert.Equal("ecos.colombia1@gmail.com", doc.RootElement.GetProperty("correo").GetString());

        var recoveryResult = await controller.SolicitarRecuperacion(new ForgotPasswordRequest
        {
            Correo = "ecos.colombia1@gmail.com",
            Rol = "Administrador",
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:R8k2M4"
        });
        var recoveryOk = Assert.IsType<OkObjectResult>(recoveryResult);
        Assert.Equal(200, recoveryOk.StatusCode);
    }

    [Fact]
    public async Task EnviarSoporte_SendsDirectlyToEcosColombia1_AndShowsSenderEmail()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var emailService = new EmailVerificationService(_config, NullLogger<EmailVerificationService>.Instance);
        var controller = CreateController(userDb, adminDb, null, emailService);

        var result = await controller.EnviarSoporte(new SupportMessageRequest
        {
            CorreoRemitente = "alejogradosena@gmail.com",
            NombreRemitente = "Alejo",
            Rol = "Usuario",
            Asunto = "Consulta rápida de soporte",
            Mensaje = "Necesito ayuda con mi cuenta en Ecos Colombia."
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        Assert.Equal("ecos.colombia1@gmail.com", doc.RootElement.GetProperty("destinatario").GetString());
        Assert.Equal("alejogradosena@gmail.com", doc.RootElement.GetProperty("remitente").GetString());
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var login = new Login { Usuario = "desconocido", contraseña = "wrongpassword", Rol = "Administrador" };
        var result = await controller.Login(login);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRequiredFieldsMissing()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var request = new RegisterRequest { Nombre = "", Correo = "", Password = "" };
        var result = await controller.Register(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailIsNotRealFormat()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var request = new RegisterRequest { Nombre = "Juan Perez", Correo = "correo-invalido", Password = "Password123*" };
        var result = await controller.Register(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenPasswordTooShort()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var request = new RegisterRequest { Nombre = "Juan Perez", Correo = "juan@test.com", Password = "123" };
        var result = await controller.Register(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsCreated_WhenValidUserProvided()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var request = new RegisterRequest
        {
            Nombre = "Juan Perez",
            Correo = "juan@test.com",
            Password = "Password123*",
            PaisOrigen = "Colombia",
            IdiomaPreferido = "ES"
        };
        var result = await controller.Register(request);

        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        var registeredUser = await userDb.usuario.FirstOrDefaultAsync(u => u.Correo == "juan@test.com");
        Assert.NotNull(registeredUser);
        Assert.Equal("Juan Perez", registeredUser.Nombre);
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenEmailAlreadyExists()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var request = new RegisterRequest
        {
            Nombre = "Juan Perez",
            Correo = "duplicado@test.com",
            Password = "Password123*"
        };
        await controller.Register(request);

        var duplicateResult = await controller.Register(request);

        var conflictResult = Assert.IsType<ConflictObjectResult>(duplicateResult);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
    }

    [Fact]
    public void Logout_ReturnsNoContent()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        var controller = CreateController(userDb, adminDb);

        var result = controller.Logout();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ValidarCodigo6Digitos_AcceptsPreviousAndResentCodes_AndHandlesSpaces()
    {
        var (userDb, adminDb) = CreateInMemoryDatabases();
        using var httpClient = new HttpClient();
        var recaptcha = new RecaptchaVerifier(httpClient, _config, NullLogger<RecaptchaVerifier>.Instance);
        var emailService = new EmailVerificationService(_config, NullLogger<EmailVerificationService>.Instance);
        var controller = CreateController(userDb, adminDb, recaptcha, emailService);

        // 1. Primer intento de Login genera el Código #1
        await controller.Login(new Login
        {
            Usuario = "turista@ecoscolombia.com",
            contraseña = "turista123",
            Rol = "Usuario"
        });
        var firstCode = AuthController.GetPendingVerificationCodeForTesting("Usuario", "turista@ecoscolombia.com");
        Assert.False(string.IsNullOrWhiteSpace(firstCode));

        // 2. El usuario solicita otro código ("Solicitar otro código" / ReenviarCodigo), lo que genera el Código #2
        await controller.ReenviarCodigo(new VerifyLoginCodeRequest
        {
            Correo = "turista@ecoscolombia.com",
            Rol = "Usuario"
        });
        var secondCode = AuthController.GetPendingVerificationCodeForTesting("Usuario", "turista@ecoscolombia.com");
        Assert.False(string.IsNullOrWhiteSpace(secondCode));

        // 3. Si el usuario ingresa el PRIMER código (incluso con espacios al copiar del correo), debe ser aceptado
        var spacedFirstCode = string.Join(" ", firstCode!.ToCharArray());
        var checkFirst = controller.ValidarCodigo6Digitos(new VerifyLoginCodeRequest
        {
            Correo = "turista@ecoscolombia.com",
            Rol = "Usuario",
            CodigoVerificacion = spacedFirstCode
        });
        Assert.IsType<OkObjectResult>(checkFirst);

        // 4. Y también puede finalizar el login con el primer código tras el reCAPTCHA
        var verifyWithFirst = await controller.VerificarCodigo(new VerifyLoginCodeRequest
        {
            Correo = "turista@ecoscolombia.com",
            Rol = "Usuario",
            CodigoVerificacion = spacedFirstCode,
            CaptchaToken = "ALPHANUMERIC_CONFIRMED:A7b9K2"
        });
        Assert.IsType<OkObjectResult>(verifyWithFirst);
    }
}
