using ApiPrueba;   
using ApiPrueba.Data;
using ApiPrueba.Dependecias;
using ApiPrueba.DBcontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://stunning-orbit-7v4vrvq6w7rp296w-5173.app.github.dev",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SQLConnectionStrings")
    ));


// Registrar DbContext + Repositorios
builder.Services.AddExternal(builder.Configuration);

// Registrar autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token)
                    && context.Request.Cookies.TryGetValue("access_token", out var accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var usuarioDb = scope.ServiceProvider.GetRequiredService<UsuarioService>();
    await usuarioDb.Database.ExecuteSqlRawAsync("""
        IF OBJECT_ID(N'[dbo].[Usuario]', N'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Usuario]
            (
                [idUsuario] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [Nombre] nvarchar(100) NOT NULL,
                [Correo] nvarchar(150) NOT NULL,
                [PaisOrigen] nvarchar(100) NULL,
                [IdiomaPreferido] nvarchar(50) NULL,
                [TipoUsuario] nvarchar(50) NULL,
                [Contraseña] nvarchar(100) NOT NULL
            );
        END
        ELSE IF COL_LENGTH(N'dbo.Usuario', N'Contraseña') IS NULL
        BEGIN
            ALTER TABLE [dbo].[Usuario] ADD [Contraseña] nvarchar(100) NULL;
        END
        IF COL_LENGTH(N'dbo.Usuario', N'Activo') IS NULL
            ALTER TABLE [dbo].[Usuario] ADD [Activo] bit NOT NULL CONSTRAINT [DF_Usuario_Activo] DEFAULT 1;
        IF COL_LENGTH(N'dbo.Usuario', N'MotivoDeshabilitacion') IS NULL
            ALTER TABLE [dbo].[Usuario] ADD [MotivoDeshabilitacion] nvarchar(500) NULL;
        IF OBJECT_ID(N'[dbo].[Resena]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Resena', N'Visible') IS NULL
            ALTER TABLE [dbo].[Resena] ADD [Visible] bit NOT NULL CONSTRAINT [DF_Resena_Visible] DEFAULT 1;
        """);

        var administradorDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await administradorDb.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[dbo].[ADMINISTRADORES]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[ADMINISTRADORES]
                (
                    [id_admin] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [nombre] nvarchar(150) NOT NULL,
                    [correo] nvarchar(150) NOT NULL,
                    [password_hash] nvarchar(100) NULL
                );
            END
            ELSE IF COL_LENGTH(N'dbo.ADMINISTRADORES', N'password_hash') IS NULL
                ALTER TABLE [dbo].[ADMINISTRADORES] ADD [password_hash] nvarchar(100) NULL;
            """);
}
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "No se pudo preparar la base de datos durante el arranque. La API iniciara, pero las operaciones que requieren SQL pueden fallar hasta recuperar la conexion.");
    }

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();