using ApiPrueba.Controllers;
using ApiPrueba.Data;
using ApiPrueba.DBcontext;
using ApiPrueba.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            { "Auth:Password", "admin123" }
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

    private AuthController CreateController(UsuarioService userDb, AppDbContext adminDb)
    {
        var controller = new AuthController(_config, userDb, adminDb)
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
}
