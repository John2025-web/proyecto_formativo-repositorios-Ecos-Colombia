using System.Data;
using System.Text.Json;
using ApiPrueba.DBcontex;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba;

public static class FrontendCatalogoMigrator
{
    private static readonly Dictionary<string, string> ContentTypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["destino"] = "Destino turístico",
        ["pagina"] = "Página",
        ["traduccion"] = "Traducción",
        ["idioma"] = "Idioma",
        ["marcadores"] = "Marcadores de mapa",
        ["descripciones"] = "Descripciones auxiliares",
        ["historia"] = "Historia",
        ["evento"] = "Evento",
        ["sitio"] = "Sitio turístico",
        ["restaurante"] = "Restaurante"
    };

    public static async Task<int> MigrateAsync(lugar_turistico_Service context)
    {
        var connection = context.Database.GetDbConnection() as SqlConnection
            ?? throw new InvalidOperationException("La migración requiere SQL Server.");
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        try
        {
            var tableExists = Convert.ToInt32(await ScalarAsync(
                connection,
                transaction,
                "SELECT CASE WHEN OBJECT_ID(N'[dbo].[FrontendCatalogo]', N'U') IS NULL THEN 0 ELSE 1 END;")) == 1;
            if (!tableExists)
            {
                await transaction.CommitAsync();
                return 0;
            }

            var entries = await ReadCatalogAsync(connection, transaction);
            var types = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var type in entries.Select(entry => entry.Tipo).Concat(["historia", "evento", "sitio", "restaurante"]).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                types[type] = await EnsureContentTypeAsync(connection, transaction, type);
            }

            var cultureCategoryId = await EnsureCultureCategoryAsync(connection, transaction, "Cultura de destino");
            var eventDescriptions = ReadDescriptionLookup(entries, "descripciones:eventos");
            var siteDescriptions = ReadDescriptionLookup(entries, "descripciones:sitios");
            var placeIds = new Dictionary<int, int>();
            var cultureIds = new Dictionary<int, int>();
            var regionIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries.Where(entry => entry.Tipo == "idioma"))
            {
                using var document = JsonDocument.Parse(entry.DatosJson);
                var code = GetString(document.RootElement, "code");
                var languageName = GetString(document.RootElement, "label");
                if (code.Length > 0 && languageName.Length > 0)
                {
                    await EnsureLanguageAsync(connection, transaction, code, languageName);
                }
            }

            var catalogContentIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries.Where(entry => entry.Tipo != "destino"))
            {
                var contentId = await EnsureContentAsync(
                    connection,
                    transaction,
                    $"Frontend:{entry.Clave} | {entry.Nombre}",
                    entry.DatosJson,
                    types[entry.Tipo]);
                catalogContentIds[entry.Clave] = contentId;
            }

            foreach (var entry in entries.Where(entry => entry.Tipo == "destino"))
            {
                using var document = JsonDocument.Parse(entry.DatosJson);
                var destination = document.RootElement;
                var destinationId = destination.GetProperty("id").GetInt32();
                var name = GetString(destination, "name");
                var region = GetString(destination, "region");
                var placeId = await EnsurePlaceAsync(connection, transaction, destination);
                placeIds[destinationId] = placeId;
                if (region.Length > 0)
                {
                    regionIds[region] = await EnsureRegionAsync(connection, transaction, region);
                }

                var cultureId = await EnsureCultureAsync(connection, transaction, name, GetString(destination, "cultura"), cultureCategoryId);
                cultureIds[destinationId] = cultureId;
                await EnsureCulturePlaceLinkAsync(connection, transaction, cultureId, placeId);

                var destinationContentId = await EnsureContentAsync(
                    connection,
                    transaction,
                    $"Frontend:{entry.Clave} | {entry.Nombre}",
                    entry.DatosJson,
                    types[entry.Tipo]);
                catalogContentIds[entry.Clave] = destinationContentId;
                await EnsureContentPlaceLinkAsync(connection, transaction, destinationContentId, placeId);
                await EnsureContentCultureLinkAsync(connection, transaction, destinationContentId, cultureId);
                await EnsureMediaAsync(connection, transaction, destinationContentId, "hero", GetString(destination, "heroImage"));

                var historyId = await EnsureContentAsync(
                    connection,
                    transaction,
                    $"Frontend:historia:{destinationId} | {name}",
                    GetString(destination, "historia"),
                    types["historia"]);
                await EnsureContentPlaceLinkAsync(connection, transaction, historyId, placeId);
                await EnsureContentCultureLinkAsync(connection, transaction, historyId, cultureId);

                if (destination.TryGetProperty("eventos", out var events))
                {
                    var eventIndex = 0;
                    foreach (var item in events.EnumerateArray())
                    {
                        eventIndex++;
                        var eventName = GetString(item, "nombre");
                        var lookupKey = $"{destinationId}:{eventName}";
                        var eventDescription = eventDescriptions.GetValueOrDefault(lookupKey, "");
                        var payload = JsonSerializer.Serialize(new
                        {
                            nombre = eventName,
                            fecha = GetString(item, "fecha"),
                            icono = GetString(item, "icono"),
                            descripcion = eventDescription
                        });
                        var eventContentId = await EnsureContentAsync(
                            connection,
                            transaction,
                            $"Frontend:evento:{destinationId}:{eventIndex} | {eventName}",
                            payload,
                            types["evento"]);
                        await EnsureContentPlaceLinkAsync(connection, transaction, eventContentId, placeId);
                        await EnsureContentCultureLinkAsync(connection, transaction, eventContentId, cultureId);

                        if (regionIds.TryGetValue(region, out var regionId))
                        {
                            var traditionId = await EnsureTraditionAsync(
                                connection,
                                transaction,
                                $"{name}: {eventName}",
                                regionId,
                                payload);
                            await EnsureCultureTraditionLinkAsync(connection, transaction, cultureId, traditionId);
                        }
                    }
                }

                if (destination.TryGetProperty("sitios", out var sites))
                {
                    var siteIndex = 0;
                    foreach (var item in sites.EnumerateArray())
                    {
                        siteIndex++;
                        var siteName = GetString(item, "nombre");
                        var lookupKey = $"{destinationId}:{siteName}";
                        var payload = JsonSerializer.Serialize(new
                        {
                            datos = item,
                            descripcion = siteDescriptions.GetValueOrDefault(lookupKey, "")
                        });
                        var siteContentId = await EnsureContentAsync(
                            connection,
                            transaction,
                            $"Frontend:sitio:{destinationId}:{siteIndex} | {siteName}",
                            payload,
                            types["sitio"]);
                        await EnsureContentPlaceLinkAsync(connection, transaction, siteContentId, placeId);
                        await EnsureMediaAsync(connection, transaction, siteContentId, "imagen", GetString(item, "foto"));
                    }
                }

                if (destination.TryGetProperty("restaurantes", out var restaurants))
                {
                    var restaurantIndex = 0;
                    foreach (var item in restaurants.EnumerateArray())
                    {
                        restaurantIndex++;
                        var restaurantName = GetString(item, "nombre");
                        var payload = JsonSerializer.Serialize(new { datos = item });
                        var restaurantContentId = await EnsureContentAsync(
                            connection,
                            transaction,
                            $"Frontend:restaurante:{destinationId}:{restaurantIndex} | {restaurantName}",
                            payload,
                            types["restaurante"]);
                        await EnsureContentPlaceLinkAsync(connection, transaction, restaurantContentId, placeId);
                        await EnsureMediaAsync(connection, transaction, restaurantContentId, "imagen", GetString(item, "foto"));
                    }
                }
            }

            await ExecuteAsync(connection, transaction, "DROP TABLE [dbo].[FrontendCatalogo];");
            await transaction.CommitAsync();
            return entries.Count;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task<List<CatalogEntry>> ReadCatalogAsync(SqlConnection connection, SqlTransaction transaction)
    {
        await using var command = new SqlCommand("""
            SELECT [Clave], [Tipo], [Nombre], [DatosJson]
            FROM [dbo].[FrontendCatalogo] WITH (TABLOCKX, HOLDLOCK)
            ORDER BY [Clave];
            """, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync();
        var entries = new List<CatalogEntry>();
        while (await reader.ReadAsync())
        {
            entries.Add(new CatalogEntry(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3)));
        }

        return entries;
    }

    private static Dictionary<string, string> ReadDescriptionLookup(List<CatalogEntry> entries, string key)
    {
        var entry = entries.FirstOrDefault(item => item.Clave == key);
        if (entry is null)
        {
            return new Dictionary<string, string>();
        }

        return JsonSerializer.Deserialize<Dictionary<string, string>>(entry.DatosJson)
            ?? new Dictionary<string, string>();
    }

    private static async Task<int> EnsureContentTypeAsync(SqlConnection connection, SqlTransaction transaction, string type)
    {
        var name = ContentTypeNames.GetValueOrDefault(type, "Contenido frontend");
        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            DECLARE @IdTipo int;
            SELECT TOP (1) @IdTipo = [IdTipo]
            FROM [dbo].[TIPO_CONTENIDO] WITH (UPDLOCK, HOLDLOCK)
            WHERE [nombre] = @Nombre;
            IF @IdTipo IS NULL
            BEGIN
                INSERT INTO [dbo].[TIPO_CONTENIDO] ([nombre]) VALUES (@Nombre);
                SET @IdTipo = CONVERT(int, SCOPE_IDENTITY());
            END
            SELECT @IdTipo;
            """, TextParameter("@Nombre", 100, name)));
    }

    private static async Task<int> EnsureCultureCategoryAsync(SqlConnection connection, SqlTransaction transaction, string name)
    {
        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            DECLARE @Id int;
            SELECT TOP (1) @Id = [id_categoria]
            FROM [dbo].[CATEGORIA_CULTURA] WITH (UPDLOCK, HOLDLOCK)
            WHERE [nombre] = @Nombre;
            IF @Id IS NULL
            BEGIN
                INSERT INTO [dbo].[CATEGORIA_CULTURA] ([nombre]) VALUES (@Nombre);
                SET @Id = CONVERT(int, SCOPE_IDENTITY());
            END
            SELECT @Id;
            """, TextParameter("@Nombre", 100, name)));
    }

    private static async Task<int> EnsureRegionAsync(SqlConnection connection, SqlTransaction transaction, string name)
    {
        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            DECLARE @Id int;
            SELECT TOP (1) @Id = [id_region]
            FROM [dbo].[REGION] WITH (UPDLOCK, HOLDLOCK)
            WHERE [nombre] = @Nombre;
            IF @Id IS NULL
            BEGIN
                INSERT INTO [dbo].[REGION] ([nombre]) VALUES (@Nombre);
                SET @Id = CONVERT(int, SCOPE_IDENTITY());
            END
            SELECT @Id;
            """, TextParameter("@Nombre", 100, name)));
    }

    private static async Task<int> EnsurePlaceAsync(SqlConnection connection, SqlTransaction transaction, JsonElement destination)
    {
        var name = GetString(destination, "name");
        var existingId = await ScalarAsync(connection, transaction, """
            SELECT TOP (1) [id_lugar]
            FROM [dbo].[LUGAR_TURISTICO] WITH (UPDLOCK, HOLDLOCK)
            WHERE [nombre] = @Nombre;
            """, TextParameter("@Nombre", 150, name));
        if (existingId is not null && existingId != DBNull.Value)
        {
            return Convert.ToInt32(existingId);
        }

        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            INSERT INTO [dbo].[LUGAR_TURISTICO] ([nombre], [ciudad], [descripcion], [imagen], [ubicacion])
            OUTPUT INSERTED.[id_lugar]
            VALUES (@Nombre, NULL, @Descripcion, @Imagen, @Ubicacion);
            """,
            TextParameter("@Nombre", 150, name),
            MaxTextParameter("@Descripcion", GetString(destination, "description")),
            TextParameter("@Imagen", 255, GetString(destination, "image")),
            TextParameter("@Ubicacion", 255, GetString(destination, "mapsQuery"))));
    }

    private static async Task<int> EnsureCultureAsync(SqlConnection connection, SqlTransaction transaction, string destination, string description, int categoryId)
    {
        var name = $"{destination} - Cultura";
        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            DECLARE @Id int;
            SELECT TOP (1) @Id = [id_cultura]
            FROM [dbo].[CULTURA] WITH (UPDLOCK, HOLDLOCK)
            WHERE [nombre] = @Nombre AND [idCategoria] = @IdCategoria;
            IF @Id IS NULL
            BEGIN
                INSERT INTO [dbo].[CULTURA] ([nombre], [descripcion], [idCategoria])
                VALUES (@Nombre, @Descripcion, @IdCategoria);
                SET @Id = CONVERT(int, SCOPE_IDENTITY());
            END
            SELECT @Id;
            """,
            TextParameter("@Nombre", 150, name),
            MaxTextParameter("@Descripcion", description),
            new SqlParameter("@IdCategoria", SqlDbType.Int) { Value = categoryId }));
    }

    private static async Task<int> EnsureContentAsync(SqlConnection connection, SqlTransaction transaction, string title, string description, int typeId)
    {
        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            DECLARE @Id int;
            SELECT TOP (1) @Id = [id_contenido]
            FROM [dbo].[CONTENIDO] WITH (UPDLOCK, HOLDLOCK)
            WHERE [titulo] = @Titulo;
            IF @Id IS NULL
            BEGIN
                INSERT INTO [dbo].[CONTENIDO] ([titulo], [descripcion], [id_tipo], [fecha_publicacion])
                VALUES (@Titulo, @Descripcion, @IdTipo, NULL);
                SET @Id = CONVERT(int, SCOPE_IDENTITY());
            END
            SELECT @Id;
            """,
            TextParameter("@Titulo", 150, title),
            MaxTextParameter("@Descripcion", description),
            new SqlParameter("@IdTipo", SqlDbType.Int) { Value = typeId }));
    }

    private static async Task<int> EnsureTraditionAsync(SqlConnection connection, SqlTransaction transaction, string name, int regionId, string description)
    {
        return Convert.ToInt32(await ScalarAsync(connection, transaction, """
            DECLARE @Id int;
            SELECT TOP (1) @Id = [id_tradicion]
            FROM [dbo].[TRADICION] WITH (UPDLOCK, HOLDLOCK)
            WHERE [nombre] = @Nombre AND [id_region] = @IdRegion;
            IF @Id IS NULL
            BEGIN
                INSERT INTO [dbo].[TRADICION] ([nombre], [id_region], [fecha_celebracion], [descripcion])
                VALUES (@Nombre, @IdRegion, NULL, @Descripcion);
                SET @Id = CONVERT(int, SCOPE_IDENTITY());
            END
            SELECT @Id;
            """,
            TextParameter("@Nombre", 150, name),
            new SqlParameter("@IdRegion", SqlDbType.Int) { Value = regionId },
            MaxTextParameter("@Descripcion", description)));
    }

    private static Task EnsureLanguageAsync(SqlConnection connection, SqlTransaction transaction, string code, string name)
    {
        return ExecuteAsync(connection, transaction, """
            IF NOT EXISTS (SELECT 1 FROM [dbo].[IDIOMA] WITH (UPDLOCK, HOLDLOCK) WHERE [codigo_iso] = @Codigo)
                INSERT INTO [dbo].[IDIOMA] ([codigo_iso], [idioma]) VALUES (@Codigo, @Nombre);
            """,
            new SqlParameter("@Codigo", SqlDbType.VarChar, 10) { Value = code },
            TextParameter("@Nombre", 50, name));
    }

    private static Task EnsureContentPlaceLinkAsync(SqlConnection connection, SqlTransaction transaction, int contentId, int placeId)
    {
        return ExecuteAsync(connection, transaction, """
            IF NOT EXISTS
            (
                SELECT 1 FROM [dbo].[CONTENIDO_LUGAR] WITH (UPDLOCK, HOLDLOCK)
                WHERE [id_contenido] = @IdContenido AND [id_lugar] = @IdLugar
            )
                INSERT INTO [dbo].[CONTENIDO_LUGAR] ([id_contenido], [id_lugar]) VALUES (@IdContenido, @IdLugar);
            """,
            new SqlParameter("@IdContenido", SqlDbType.Int) { Value = contentId },
            new SqlParameter("@IdLugar", SqlDbType.Int) { Value = placeId });
    }

    private static Task EnsureContentCultureLinkAsync(SqlConnection connection, SqlTransaction transaction, int contentId, int cultureId)
    {
        return ExecuteAsync(connection, transaction, """
            IF NOT EXISTS
            (
                SELECT 1 FROM [dbo].[CONTENIDO_CULTURA] WITH (UPDLOCK, HOLDLOCK)
                WHERE [id_contenido] = @IdContenido AND [id_cultura] = @IdCultura
            )
                INSERT INTO [dbo].[CONTENIDO_CULTURA] ([id_contenido], [id_cultura]) VALUES (@IdContenido, @IdCultura);
            """,
            new SqlParameter("@IdContenido", SqlDbType.Int) { Value = contentId },
            new SqlParameter("@IdCultura", SqlDbType.Int) { Value = cultureId });
    }

    private static Task EnsureCulturePlaceLinkAsync(SqlConnection connection, SqlTransaction transaction, int cultureId, int placeId)
    {
        return ExecuteAsync(connection, transaction, """
            IF NOT EXISTS
            (
                SELECT 1 FROM [dbo].[CulturaLugar] WITH (UPDLOCK, HOLDLOCK)
                WHERE [idCultura] = @IdCultura AND [idLugar] = @IdLugar
            )
                INSERT INTO [dbo].[CulturaLugar] ([idCultura], [idLugar]) VALUES (@IdCultura, @IdLugar);
            """,
            new SqlParameter("@IdCultura", SqlDbType.Int) { Value = cultureId },
            new SqlParameter("@IdLugar", SqlDbType.Int) { Value = placeId });
    }

    private static Task EnsureCultureTraditionLinkAsync(SqlConnection connection, SqlTransaction transaction, int cultureId, int traditionId)
    {
        return ExecuteAsync(connection, transaction, """
            IF NOT EXISTS
            (
                SELECT 1 FROM [dbo].[CULTURA_TRADICION] WITH (UPDLOCK, HOLDLOCK)
                WHERE [id_cultura] = @IdCultura AND [id_tradicion] = @IdTradicion
            )
                INSERT INTO [dbo].[CULTURA_TRADICION] ([id_cultura], [id_tradicion]) VALUES (@IdCultura, @IdTradicion);
            """,
            new SqlParameter("@IdCultura", SqlDbType.Int) { Value = cultureId },
            new SqlParameter("@IdTradicion", SqlDbType.Int) { Value = traditionId });
    }

    private static Task EnsureMediaAsync(SqlConnection connection, SqlTransaction transaction, int contentId, string type, string url)
    {
        if (url.Length == 0)
        {
            return Task.CompletedTask;
        }

        return ExecuteAsync(connection, transaction, """
            IF NOT EXISTS
            (
                SELECT 1 FROM [dbo].[MULTIMEDIA] WITH (UPDLOCK, HOLDLOCK)
                WHERE [id_contenido] = @IdContenido AND [url] = @Url
            )
                INSERT INTO [dbo].[MULTIMEDIA] ([id_contenido], [tipo], [url]) VALUES (@IdContenido, @Tipo, @Url);
            """,
            new SqlParameter("@IdContenido", SqlDbType.Int) { Value = contentId },
            TextParameter("@Tipo", 50, type),
            TextParameter("@Url", 255, url));
    }

    private static async Task<object?> ScalarAsync(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
    {
        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddRange(parameters);
        return await command.ExecuteScalarAsync();
    }

    private static async Task ExecuteAsync(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
    {
        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddRange(parameters);
        await command.ExecuteNonQueryAsync();
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";
    }

    private static SqlParameter TextParameter(string name, int size, string value)
    {
        return new SqlParameter(name, SqlDbType.NVarChar, size) { Value = value };
    }

    private static SqlParameter MaxTextParameter(string name, string value)
    {
        return new SqlParameter(name, SqlDbType.NVarChar, -1) { Value = value };
    }

    private sealed record CatalogEntry(string Clave, string Tipo, string Nombre, string DatosJson);
}