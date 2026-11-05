using ApiPrueba.Controllers;
using ApiPrueba.models;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiPrueba.Tests;

public class CultuLugarControllerTests
{
    private static readonly Func<CultuLugarController, Task<IActionResult>>[] CrudOperations =
    [
        controller => controller.GetculturaLugar(),
        controller => controller.obtenerId(1),
        controller => controller.PostculturaLugar(CreateCulturaLugar()),
        controller => controller.PutculturaLugar(CreateCulturaLugar()),
        controller => controller.DeleteculturaLugar(CreateCulturaLugar())
    ];

    public static IEnumerable<object[]> CrudActions =>
        CrudOperations.Select(operation => new object[] { operation });

    public static IEnumerable<object[]> CrudErrors =>
    [
        .. CrudOperations.Select(operation => new object[] { operation, new UnauthorizedAccessException(), 401 }),
        .. CrudOperations.Select(operation => new object[] { operation, new ArgumentException("Entrada inválida"), 400 }),
        .. CrudOperations.Select(operation => new object[] { operation, new InvalidOperationException("Fallo interno"), 500 })
    ];

    [Theory]
    [MemberData(nameof(CrudActions))]
    public async Task Crud_ReturnsOk_WhenRepositorySucceeds(
        Func<CultuLugarController, Task<IActionResult>> execute)
    {
        var controller = CreateController(new FakeCulturaLugarRepository());

        var result = await execute(controller);

        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [MemberData(nameof(CrudErrors))]
    public async Task Crud_MapsRepositoryExceptionsToHttpStatus(
        Func<CultuLugarController, Task<IActionResult>> execute,
        Exception error,
        int expectedStatus)
    {
        var repository = new FakeCulturaLugarRepository { Error = error };
        var controller = CreateController(repository);

        var result = await execute(controller);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(expectedStatus, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsBadRequest_WhenNotFound()
    {
        var repository = new FakeCulturaLugarRepository { ReturnNull = true };
        var controller = CreateController(repository);

        var result = await controller.obtenerId(999);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, statusResult.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenRepositoryFails()
    {
        var repository = new FakeCulturaLugarRepository { ReturnFalse = true };
        var controller = CreateController(repository);

        var result = await controller.PostculturaLugar(CreateCulturaLugar());

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, statusResult.StatusCode);
    }

    private static CultuLugarController CreateController(IculturaLugarRepo repository) => new(repository);

    private static culturaLugar CreateCulturaLugar() => new()
    {
        idCultura = 1,
        idLugar = 1
    };

    private sealed class FakeCulturaLugarRepository : IculturaLugarRepo
    {
        public Exception? Error { get; init; }
        public bool ReturnNull { get; init; }
        public bool ReturnFalse { get; init; }

        public Task<List<culturaLugar>> GetculturaLugar()
        {
            ThrowIfConfigured();
            return Task.FromResult(ReturnNull ? null! : new List<culturaLugar> { CreateCulturaLugar() });
        }

        public Task<culturaLugar?> GetculturaLugarById(int id)
        {
            ThrowIfConfigured();
            return Task.FromResult(ReturnNull ? null : CreateCulturaLugar());
        }

        public Task<bool> PostculturaLugar(culturaLugar culturaLugar)
        {
            ThrowIfConfigured();
            return Task.FromResult(!ReturnFalse);
        }

        public Task<bool> PutculturaLugar(culturaLugar culturaLugar)
        {
            ThrowIfConfigured();
            return Task.FromResult(!ReturnFalse);
        }

        public Task<bool> DeleteculturaLugar(culturaLugar culturaLugar)
        {
            ThrowIfConfigured();
            return Task.FromResult(!ReturnFalse);
        }

        private void ThrowIfConfigured()
        {
            if (Error is not null)
            {
                throw Error;
            }
        }
    }
}
