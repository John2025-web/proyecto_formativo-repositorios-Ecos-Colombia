using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiPrueba.Tests;

public class RepositoryCrudTests
{
    public static IEnumerable<object[]> RepositoryTypes =>
        FindCrudRepositories().Select(repository => new object[] { repository.FullName! });

    [Theory]
    [MemberData(nameof(RepositoryTypes))]
    public async Task Repository_PersistsCreateReadUpdateAndDelete(string repositoryName)
    {
        var assembly = typeof(ApiPrueba.models.CULTURA).Assembly;
        var repositoryType = assembly.GetType(repositoryName, throwOnError: true)!;
        var contextType = repositoryType.GetConstructors()
            .Single(constructor => constructor.GetParameters().Length == 1)
            .GetParameters()[0].ParameterType;
        var context = CreateContext(contextType);
        await using var disposeContext = context;
        var repository = Activator.CreateInstance(repositoryType, context)!;
        var entityType = repositoryType.GetMethods()
            .Where(method => method.Name.StartsWith("Post", StringComparison.Ordinal))
            .Select(method => method.GetParameters().SingleOrDefault()?.ParameterType)
            .First(type => type is not null && type.IsClass && type != typeof(string))!;
        var entity = CreateEntity(entityType);

        var create = FindEntityOperation(repositoryType, "Post", entityType);
        var update = FindEntityOperation(repositoryType, "Put", entityType);
        var delete = FindEntityOperation(repositoryType, "Delete", entityType);
        var list = FindListOperation(repositoryType, entityType);
        var find = FindByIdOperation(repositoryType, entityType);

        Assert.True(await InvokeAsync<bool>(repository, create, entity));
        Assert.Single((IEnumerable)(await InvokeAsync<object>(repository, list))!);

        var saved = await InvokeAsync<object>(repository, find, CreateLookupArguments(find, context, entity));
        Assert.NotNull(saved);
        var primaryKeyNames = context.Model.FindEntityType(entityType)!.FindPrimaryKey()!.Properties
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
        var propertyToUpdate = entityType.GetProperties().FirstOrDefault(property =>
            property.CanWrite && !primaryKeyNames.Contains(property.Name) &&
            property.PropertyType is { } type &&
            (type == typeof(string) || type == typeof(bool) || type == typeof(int) ||
             type == typeof(decimal) || type == typeof(DateTime)));
        object? updatedValue = null;
        if (propertyToUpdate is not null)
        {
            var currentValue = propertyToUpdate.GetValue(saved);
            updatedValue = propertyToUpdate.PropertyType == typeof(string) ? "Registro actualizado"
                : propertyToUpdate.PropertyType == typeof(bool) ? !(bool)currentValue!
                : propertyToUpdate.PropertyType == typeof(int) ? (int)currentValue! + 1
                : propertyToUpdate.PropertyType == typeof(decimal) ? (decimal)currentValue! + 1m
                : DateTime.UtcNow;
            propertyToUpdate.SetValue(saved, updatedValue);
        }
        Assert.True(await InvokeAsync<bool>(repository, update, saved));
        var updated = await InvokeAsync<object>(repository, find, CreateLookupArguments(find, context, saved));
        Assert.NotNull(updated);
        if (propertyToUpdate is not null)
        {
            Assert.Equal(updatedValue, propertyToUpdate.GetValue(updated));
        }

        Assert.True(await InvokeAsync<bool>(repository, delete, saved));
        Assert.Empty((IEnumerable)(await InvokeAsync<object>(repository, list))!);
    }

    private static IEnumerable<Type> FindCrudRepositories() =>
        typeof(ApiPrueba.models.CULTURA).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetConstructors().Any(constructor =>
                constructor.GetParameters() is [{ ParameterType: var parameterType }] &&
                typeof(DbContext).IsAssignableFrom(parameterType)))
            .Where(type => type.GetMethods().Any(method =>
                method.Name.StartsWith("Post", StringComparison.Ordinal)) &&
                type.GetMethods().Any(method => method.Name.StartsWith("Put", StringComparison.Ordinal)) &&
                type.GetMethods().Any(method => method.Name.StartsWith("Delete", StringComparison.Ordinal)));

    private static DbContext CreateContext(Type contextType)
    {
        var create = typeof(RepositoryCrudTests).GetMethod(
            nameof(CreateContextOfType), BindingFlags.NonPublic | BindingFlags.Static)!;
        return (DbContext)create.MakeGenericMethod(contextType).Invoke(null, null)!;
    }

    private static TContext CreateContextOfType<TContext>() where TContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }

    private static object CreateEntity(Type entityType)
    {
        var entity = Activator.CreateInstance(entityType)!;
        foreach (var property in entityType.GetProperties().Where(property => property.CanWrite))
        {
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            object? value = propertyType == typeof(string) ? "Registro de prueba"
                : propertyType == typeof(int) ? 1
                : propertyType == typeof(long) ? 1L
                : propertyType == typeof(bool) ? true
                : propertyType == typeof(decimal) ? 1m
                : propertyType == typeof(double) ? 1d
                : propertyType == typeof(float) ? 1f
                : propertyType == typeof(Guid) ? Guid.NewGuid()
                : propertyType == typeof(DateTime) ? DateTime.UtcNow
                : propertyType == typeof(DateOnly) ? DateOnly.FromDateTime(DateTime.UtcNow)
                : propertyType == typeof(TimeOnly) ? TimeOnly.FromDateTime(DateTime.UtcNow)
                : propertyType == typeof(byte[]) ? Array.Empty<byte>()
                : propertyType.IsEnum ? Enum.GetValues(propertyType).GetValue(0)
                : null;

            if (value is not null)
            {
                property.SetValue(entity, value);
            }
        }

        return entity;
    }

    private static MethodInfo FindEntityOperation(Type repositoryType, string prefix, Type entityType) =>
        repositoryType.GetMethods().First(method =>
            method.Name.StartsWith(prefix, StringComparison.Ordinal) &&
            method.GetParameters() is [{ ParameterType: var parameterType }] &&
            parameterType == entityType);

    private static MethodInfo FindListOperation(Type repositoryType, Type entityType) =>
        repositoryType.GetMethods().Single(method =>
            method.Name.StartsWith("Get", StringComparison.Ordinal) &&
            method.GetParameters().Length == 0 &&
            GetTaskResultType(method.ReturnType) == typeof(List<>).MakeGenericType(entityType));

    private static MethodInfo FindByIdOperation(Type repositoryType, Type entityType) =>
        repositoryType.GetMethods().Single(method =>
            method.Name.StartsWith("Get", StringComparison.Ordinal) &&
            method.GetParameters().Length > 0 &&
            GetTaskResultType(method.ReturnType) == entityType);

    private static Type? GetTaskResultType(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)
            ? type.GetGenericArguments()[0]
            : null;

    private static object?[] CreateLookupArguments(MethodInfo method, DbContext context, object entity)
    {
        var keyProperties = context.Model.FindEntityType(entity.GetType())?.FindPrimaryKey()?.Properties
            ?? throw new InvalidOperationException($"No se encontró la clave primaria de {entity.GetType().Name}.");
        var keyValues = keyProperties.Select(property => context.Entry(entity).Property(property.Name).CurrentValue).ToArray();

        return method.GetParameters().Select((parameter, index) =>
        {
            var keyIndex = -1;
            for (var candidateIndex = 0; candidateIndex < keyProperties.Count; candidateIndex++)
            {
                if (string.Equals(keyProperties[candidateIndex].Name, parameter.Name, StringComparison.OrdinalIgnoreCase))
                {
                    keyIndex = candidateIndex;
                    break;
                }
            }

            if (keyIndex < 0 && index < keyValues.Length)
            {
                keyIndex = index;
            }

            return keyIndex >= 0 && keyIndex < keyValues.Length ? keyValues[keyIndex] : null;
        }).ToArray();
    }

    private static async Task<TResult?> InvokeAsync<TResult>(object instance, MethodInfo method, params object?[] arguments)
    {
        var task = (Task)method.Invoke(instance, arguments)!;
        await task;
        return (TResult?)task.GetType().GetProperty("Result")?.GetValue(task);
    }
}