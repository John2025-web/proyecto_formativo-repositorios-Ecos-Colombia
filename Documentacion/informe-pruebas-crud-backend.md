# Informe de pruebas CRUD del backend

**Fecha de ejecución:** 21 de octubre de 2026
**Proyecto:** `backend-visual` (`net10.0`)
**Suite:** `backend-visual.Tests` con xUnit y EF Core InMemory

## Resumen ejecutivo

La ejecución completa aprobó **40 de 40 pruebas**: 22 pruebas del comportamiento HTTP de `CULTURAController` y 18 ciclos CRUD de repositorios EF Core, uno por cada repositorio encontrado con operaciones de creación, consulta, actualización y eliminación.

| Resultado | Cantidad |
| --- | ---: |
| Pruebas ejecutadas | 40 |
| Aprobadas | 40 |
| Fallidas | 0 |
| Omitidas | 0 |
| Repositorios con ciclo CRUD probado | 18 |

## Cobertura por tabla

Cada prueba de repositorio utiliza una instancia EF Core InMemory aislada y ejecuta crear, listar, consultar por clave, actualizar y eliminar.

| Entidad / tabla | Repositorio | Resultado |
| --- | --- | --- |
| `ADMINISTRADORES` | `ADMINISTRADORESRepository` | Aprobado |
| `CATEGORIA_CULTURA` | `CATEGORIA_CULTURARepository` | Aprobado |
| `CONTENIDO_CULTURA` | `CONTENIDO_CULTURARepository` | Aprobado |
| `CONTENIDO_LUGAR` | `CONTENIDO_LUGARRepository` | Aprobado |
| `CONTENIDO` | `CONTENIDORepository` | Aprobado |
| `CULTURA` | `CULTURARepository` | Aprobado |
| `CULTURA-LUGAR` | `CulturaLugarRepository` | Aprobado |
| `CULTURA-TRADICION` | `culturatradiRepository` | Aprobado |
| `GASTRONOMIA` | `gastroRepository` | Aprobado |
| `IDIOMA` | `IdiomaRepository` | Aprobado |
| `LUGAR_TURISTICO` | `LugarTuRepository` | Aprobado |
| `MULTIMEDIA` | `MultimeRepository` | Aprobado |
| `Notificacion` | `NotificacionRepository` | Aprobado |
| `Resena` | `RecenaRepository` | Aprobado |
| `Region` | `RegionRepositoy` | Aprobado |
| `Tipo_Contenido` | `Tipo_ContenidoRepository` | Aprobado |
| `Tradicion` | `TradicionRepository` | Aprobado |
| `Usuario` | `UsuarioRepository` | Aprobado |

Además, las 22 pruebas de `CULTURAController` validan cinco operaciones ante éxito, excepciones (`401`, `400` y `500`) y respuestas nulas en las consultas.

## Ejecución y cobertura

Comando de pruebas:

```bash
dotnet test backend-visual.Tests/ApiPrueba.Tests.csproj --logger "console;verbosity=minimal"
```

Comando de cobertura OpenCover:

```bash
dotnet test backend-visual.Tests/ApiPrueba.Tests.csproj --no-build --logger "console;verbosity=minimal" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults/coverage/
```

La cobertura agregada informada para el ensamblado `ApiPrueba` fue **21,26% de líneas**, **1,25% de ramas** y **55,85% de métodos**. Es una métrica global del ensamblado, no una cobertura individual de cada tabla ni controlador. El bajo porcentaje de ramas muestra que aún faltan escenarios alternos.

## Hallazgo corregido durante las pruebas

Las pruebas descubrieron y se corrigieron tres problemas de mapeo EF Core:

- `Usuario.idUsuario` no se reconocía como clave primaria, lo que impedía construir `NotificacionService`. Se agregó `[Key]` en `Models/Usuario.cs`.
- `MultimediaService` asignaba dos claves y nombres de tabla sucesivos a la misma entidad. Se dejó `MULTIMEDIA` como tabla, `IdMultimedia` como clave y se alinearon `id_contenido` y `tipo` con las columnas usadas por el migrador del catálogo.
- `GastronomiaService` usaba nombres de columnas como nombres de tabla y configuraba `IdRegion` como clave. Se alineó el mapa con `GASTRONOMIA` e `IdPlato`, que es la clave consultada por el repositorio.
- `cultura_TradicionService` asignaba nombres de tablas separados a una sola entidad. Se configuró `CULTURA_TRADICION` y su clave compuesta `idCultura`/`idTradicion`, consistente con el migrador del catálogo.

Las 18 pruebas CRUD pasan con estos cambios. Se recomienda contrastar los nombres físicos de tabla y columna con el esquema desplegado antes de aplicar migraciones en producción.

## Hallazgos pendientes

1. `dotnet list backend-visual/ApiPrueba.csproj package --vulnerable --include-transitive` reportó `System.Security.Cryptography.Pkcs` versión `9.0.4` como dependencia transitiva vulnerable. NuGet mostró severidad alta y moderada: [GHSA-846f-f764-76mx](https://github.com/advisories/GHSA-846f-f764-76mx) y [GHSA-2fmv-rv2c-59x7](https://github.com/advisories/GHSA-2fmv-rv2c-59x7). La dependencia no se actualizó.
2. No se encontró configuración local de SonarCloud; por tanto, no se ejecutó análisis ni se reportan hallazgos de SonarCloud.

## Alcance y limitaciones

- Las 18 pruebas ejecutan los repositorios reales contra EF Core InMemory; no conectan con SQL Server ni alteran bases de datos existentes.
- El proveedor InMemory no reproduce todas las restricciones y el comportamiento relacional de SQL Server. Se recomienda validar esquema, claves foráneas y migraciones con una base SQL de pruebas antes de considerar completa la verificación de persistencia.
- Solo `CULTURAController` tiene pruebas directas de sus respuestas HTTP. Los otros controladores no se probaron mediante el pipeline ASP.NET Core, autorización, serialización ni rutas HTTP.
- Los casos CRUD usan fixtures genéricos. No cubren todas las reglas de negocio ni todos los valores límite de cada entidad.
- La cobertura de ramas es baja y la cifra agregada no identifica por sí sola qué entidad necesita más pruebas.

## Próximos pasos recomendados

1. Añadir pruebas HTTP de los controladores restantes, especialmente autenticación, validación y autorización.
2. Ejecutar pruebas de integración contra SQL Server aislado para verificar relaciones, restricciones y compatibilidad real del esquema.
3. Revisar y actualizar la dependencia transitiva vulnerable a una versión corregida compatible.
4. Configurar SonarCloud en CI e importar OpenCover usando `sonar.cs.opencover.reportsPaths=backend-visual.Tests/TestResults/coverage/coverage.opencover.xml`.
5. Revisar las advertencias de compilación y los resultados de SonarCloud por prioridad, sin tratar la cobertura como sustituto del análisis estático.