using ApiPrueba.Controllers;
using ApiPrueba.models;
using ApiPrueba.Repositories.@interface;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiPrueba.Tests;

public class CULTURAControllerTests
{
    private static readonly Func<CULTURAController, Task<IActionResult>>[] CrudOperations =
    [
        controller => controller.GetCulturas(),
        controller => controller.ObtenerId(1),
        controller => controller.PostCultura(CreateCultura()),
        controller => controller.PutCultura(CreateCultura()),
        controller => controller.DeleteCultura(CreateCultura())
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
        Func<CULTURAController, Task<IActionResult>> execute)
    {
        var controller = CreateController(new FakeCulturaRepository());

        var result = await execute(controller);

        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [MemberData(nameof(CrudErrors))]
    public async Task Crud_MapsRepositoryExceptionsToHttpStatus(
        Func<CULTURAController, Task<IActionResult>> execute,
        Exception error,
        int expectedStatus)
    {
        var repository = new FakeCulturaRepository { Error = error };
        var controller = CreateController(repository);

        var result = await execute(controller);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(expectedStatus, statusResult.StatusCode);
    }

    [Theory]
    [MemberData(nameof(ReadActions))]
    public async Task Read_ReturnsBadRequest_WhenRepositoryReturnsNull(
        Func<CULTURAController, Task<IActionResult>> execute)
    {
        var repository = new FakeCulturaRepository { ReturnNull = true };
        var controller = CreateController(repository);

        var result = await execute(controller);

        Assert.Equal(400, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    public static IEnumerable<object[]> ReadActions =>
    [
        [CrudOperations[0]],
        [CrudOperations[1]]
    ];

    private static CULTURAController CreateController(ICULTURARepository repository) => new(repository);

    private static CULTURA CreateCultura() => new()
    {
        IdCultura = 1,
        Nombre = "Cultura de prueba",
        Descripcion = "Descripción de prueba",
        IdCategoria = 1
    };

    private sealed class FakeCulturaRepository : ICULTURARepository
    {
        public Exception? Error { get; init; }
        public bool ReturnNull { get; init; }

        public Task<List<CULTURA>> GetCulturas()
        {
            ThrowIfConfigured();
            return Task.FromResult(ReturnNull ? null! : new List<CULTURA> { CreateCultura() });
        }

        public Task<CULTURA> GetCulturaById(int id)
        {
            ThrowIfConfigured();
            return Task.FromResult(ReturnNull ? null! : CreateCultura());
        }

        public Task<bool> PostCultura(CULTURA cultura)
        {
            ThrowIfConfigured();
            return Task.FromResult(true);
        }

        public Task<bool> PutCultura(CULTURA cultura)
        {
            ThrowIfConfigured();
            return Task.FromResult(true);
        }

        public Task<bool> DeleteCultura(CULTURA cultura)
        {
            ThrowIfConfigured();
            return Task.FromResult(true);
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