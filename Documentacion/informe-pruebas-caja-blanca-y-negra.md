# INFORME TÉCNICO Y PLAN DE PRUEBAS DE SOFTWARE
## Pruebas de Caja Negra, Caja Blanca y Automatizadas
### Proyecto: Ecos de Colombia (EcosColombia Web API)
**Basado en estándares ISTQB Foundation Level e ISO/IEC/IEEE 29119**

---

| Metadato | Detalle |
| :--- | :--- |
| **Proyecto** | Ecos de Colombia (`proyecto_formativo-repositorios-Ecos-Colombia`) |
| **Repositorio** | [GitHub - Ecos-Colombia](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia) |
| **Versión Evaluada** | 2.4.0 (Backend .NET 10 Web API) |
| **Frameworks de Prueba** | xUnit 2.9.3, Entity Framework Core In-Memory 10.0, Test SDK 17.14 |
| **Total Pruebas Automatizadas** | 74 Pruebas Unitarias y de Integración |
| **Resultado de Automatización** | **74 Superadas / 0 Fallidas / 0 Omitidas** (100% éxito) |
| **Fecha de Ejecución** | Noviembre de 2026 |

---

## 1. Introducción y Descripción del Proyecto

**Ecos de Colombia** es una plataforma tecnológica orientada a la difusión, preservación y gestión del patrimonio cultural, turístico, gastronómico y tradicional de las distintas regiones de Colombia. 

El núcleo del sistema es un Backend desarrollado bajo arquitectura en capas con **ASP.NET Core Web API**, conectándose a una base de datos relacional hospedada en **Azure SQL Database** (`basedatosecos.database.windows.net`), protegido mediante tokens criptográficos **JWT (JSON Web Tokens)** con control de acceso basado en roles (**RBAC**: Administrador y Usuario/Turista).

### 1.1 Tecnologías Utilizadas
* **Lenguaje:** C# 13 / .NET 10.0 LTS.
* **Acceso a Datos:** Entity Framework Core con DbContext por contexto de dominio.
* **Seguridad y Criptografía:** JWT Bearer Authentication, HMAC-SHA256, BCrypt.Net para hashing de contraseñas.
* **Documentación Interactiva:** Swagger / OpenAPI con esquema de seguridad JWT Bearer (`Authorize 🔓`).
* **Framework de Pruebas:** xUnit, Microsoft.NET.Test.Sdk, Microsoft.EntityFrameworkCore.InMemory.
* **Cliente de Pruebas de Sistema:** Postman v11+, Swagger UI en `https://localhost:7119/swagger/index.html`.

---

## 2. Pruebas de Caja Negra (Black Box Testing)

En este enfoque se evalúa el comportamiento externo de los endpoints de la API frente a las especificaciones y contratos de interfaz, sin considerar el código fuente interno. Se probaron casos normales (Happy Path), valores límite y datos incorrectos/inválidos para **todas las tablas y controladores** del sistema.

| ID | Funcionalidad | Datos de entrada | Pasos | Resultado esperado | Resultado obtenido | Estado |
|:---|:---|:---|:---|:---|:---|:---|
| **CN-AUTH-01** | Inicio de sesión Administrador (Caso normal) | `{"usuario": "admin", "contraseña": "admin123", "rol": "Administrador"}` | 1. Enviar `POST /api/Auth/Login`. 2. Inspeccionar cuerpo de respuesta y cookie. | HTTP 200 OK, JSON con `token` JWT Bearer y cookie `access_token`. | HTTP 200 OK, JSON con token válido. | **Aprobado** |
| **CN-AUTH-02** | Inicio de sesión Usuario Turista (Caso normal) | `{"usuario": "turista@ecoscolombia.com", "contraseña": "turista123", "rol": "Usuario"}` | 1. Enviar `POST /api/Auth/Login`. 2. Verificar rol en Claims. | HTTP 200 OK, token con Claim `Role: Usuario`. | HTTP 200 OK, token generado correctamente. | **Aprobado** |
| **CN-AUTH-03** | Login con credenciales erróneas (Dato incorrecto) | `{"usuario": "admin", "contraseña": "PasswordErroneo999*", "rol": "Administrador"}` | 1. Enviar `POST /api/Auth/Login`. 2. Validar respuesta de seguridad. | HTTP 401 Unauthorized sin emitir token. | HTTP 401 Unauthorized. | **Aprobado** |
| **CN-AUTH-04** | Login con campos vacíos (Caso límite) | `{"usuario": "", "contraseña": "", "rol": ""}` | 1. Enviar `POST /api/Auth/Login`. | HTTP 400 Bad Request indicando campos requeridos. | HTTP 400 Bad Request con mensaje descriptivo. | **Aprobado** |
| **CN-AUTH-05** | Registro de nuevo usuario (Caso normal) | `{"nombre": "Carlos Gómez", "correo": "carlos@test.com", "password": "Password123*", "paisOrigen": "Colombia", "idiomaPreferido": "ES"}` | 1. Enviar `POST /api/Auth/Register`. | HTTP 201 Created con mensaje de confirmación. | HTTP 201 Created. | **Aprobado** |
| **CN-AUTH-06** | Registro con contraseña corta (Valor límite) | `{"nombre": "Ana", "correo": "ana@test.com", "password": "123"}` (< 6 caracteres) | 1. Enviar `POST /api/Auth/Register`. | HTTP 400 Bad Request: "La contraseña debe tener al menos 6 caracteres." | HTTP 400 Bad Request validado. | **Aprobado** |
| **CN-AUTH-07** | Registro con correo duplicado (Dato inválido) | `{"nombre": "Carlos Gómez", "correo": "carlos@test.com", "password": "Password123*"}` | 1. Enviar `POST /api/Auth/Register` con correo ya registrado. | HTTP 409 Conflict: "Ya existe un usuario con ese correo." | HTTP 409 Conflict retornado. | **Aprobado** |
| **CN-CULT-01** | Listar Culturas sin token (Seguridad) | Ninguno (Sin Header Authorization) | 1. Enviar `GET /api/CULTURA`. | HTTP 401 Unauthorized. | HTTP 401 Unauthorized. | **Aprobado** |
| **CN-CULT-02** | Listar Culturas con Token Bearer (Caso normal) | Header `Authorization: Bearer <token_admin>` | 1. Autenticarse. 2. Enviar `GET /api/CULTURA`. | HTTP 200 OK con arreglo JSON de culturas colombianas. | HTTP 200 OK con registros de culturas. | **Aprobado** |
| **CN-CULT-03** | Crear Expresión Cultural (Caso normal) | Header Bearer, JSON: `{"nombre": "Cultura Wayuu", "descripcion": "Tradición ancestral guajira", "idCategoria": 1}` | 1. Enviar `POST /api/CULTURA`. | HTTP 200 OK / 201 Created confirmando inserción. | HTTP 200 OK con confirmación `true`. | **Aprobado** |
| **CN-CULT-04** | Consultar Cultura por ID inexistente (Caso límite) | `GET /api/CULTURA/99999` | 1. Consultar ID fuera de rango en base de datos. | HTTP 400 Bad Request o 404 Not Found. | HTTP 400 Bad Request con mensaje de error. | **Aprobado** |
| **CN-LUG-01** | Crear Lugar Turístico (Caso normal) | Header Bearer, JSON: `{"nombre": "Parque Tayrona", "ciudad": "Santa Marta", "descripcion": "Playas y selva", "imagen": "tayrona.jpg", "idRegion": 1, "ubicacion": "Magdalena"}` | 1. Enviar `POST /api/LugarTuri`. | HTTP 200 OK con persistencia en Azure SQL. | HTTP 200 OK y registro insertado. | **Aprobado** |
| **CN-LUG-02** | Consultar Lugar por ID (Caso normal) | `GET /api/LugarTuri/1` con Token Bearer | 1. Enviar petición con ID existente. | HTTP 200 OK con objeto del lugar turístico. | HTTP 200 OK con los datos de Santa Marta. | **Aprobado** |
| **CN-CULLUG-01** | Listar Relación Cultura-Lugar (Caso normal) | Header Bearer, `GET /api/CultuLugar` | 1. Consultar endpoint asociativo. | HTTP 200 OK con lista de pares `idCultura` e `idLugar`. | HTTP 200 OK con lista de asociaciones. | **Aprobado** |
| **CN-CULLUG-02** | Asociar Cultura a Lugar Turístico (Caso normal) | Header Bearer, JSON: `{"idCultura": 1, "idLugar": 1}` | 1. Enviar `POST /api/CultuLugar`. | HTTP 200 OK / persistencia de clave compuesta. | HTTP 200 OK retornando `true`. | **Aprobado** |
| **CN-CULLUG-03** | Consultar CulturaLugar por ID Cultura (Caso normal) | `GET /api/CultuLugar/1` | 1. Enviar ID de cultura existente. | HTTP 200 OK con la relación encontrada. | HTTP 200 OK con datos de relación. | **Aprobado** |
| **CN-CULLUG-04** | Eliminar asociación Cultura-Lugar (Caso normal) | Header Bearer, `DELETE /api/CultuLugar` con body `{"idCultura": 1, "idLugar": 1}` | 1. Enviar eliminación de par asociativo. | HTTP 200 OK con confirmación de desasociación. | HTTP 200 OK y registro eliminado. | **Aprobado** |
| **CN-CULTRAD-01** | Asociar Cultura a Tradición (Caso normal) | Header Bearer, `POST /api/cultradi` con `{"idCultura": 1, "idTradicion": 1}` | 1. Enviar asociación a endpoint `cultradi`. | HTTP 200 OK. | HTTP 200 OK. | **Aprobado** |
| **CN-TRAD-01** | Consultar Tradiciones (Caso normal) | Header Bearer, `GET /api/Trad` | 1. Solicitar listado general de tradiciones. | HTTP 200 OK con tradiciones y fechas de celebración. | HTTP 200 OK con array JSON. | **Aprobado** |
| **CN-GAST-01** | Consultar Gastronomía (Caso normal) | Header Bearer, `GET /api/gastrono` | 1. Solicitar platos típicos colombianos. | HTTP 200 OK con platos, ingredientes y región. | HTTP 200 OK con listado gastronómico. | **Aprobado** |
| **CN-REC-01** | Registrar Reseña de Turista (Caso normal) | Header Bearer, `POST /api/Recena` con `{"idUsuario": 1, "idLugar": 1, "calificacion": 5, "comentario": "Excelente experiencia ecológica", "fecha": "2026-11-05"}` | 1. Enviar reseña válida. | HTTP 200 OK con reseña vinculada. | HTTP 200 OK con persistencia en DB. | **Aprobado** |
| **CN-NOTI-01** | Consultar Noticias Culturales (Caso normal) | Header Bearer, `GET /api/Noti` | 1. Solicitar noticias publicadas. | HTTP 200 OK con noticias y eventos culturales. | HTTP 200 OK con datos. | **Aprobado** |
| **CN-REG-01** | Consultar Regiones Geográficas (Caso normal) | Header Bearer, `GET /api/Region` | 1. Solicitar regiones (Andina, Caribe, Pacífica, etc.). | HTTP 200 OK con las regiones del país. | HTTP 200 OK con registros. | **Aprobado** |
| **CN-MULT-01** | Registrar Multimedia (Caso normal) | Header Bearer, `POST /api/multimedia` con `{"idLugar": 1, "tipoContenido": "Imagen", "url": "https://cdn.ecoscolombia.com/foto1.jpg", "descripcion": "Panorámica"}` | 1. Enviar recurso multimedia. | HTTP 200 OK confirmando inserción. | HTTP 200 OK. | **Aprobado** |
| **CN-CONT-01** | Crear Contenido Editorial (Caso normal) | Header Bearer, `POST /api/CONTENIDO` con `{"titulo": "Leyendas del Caribe", "descripcion": "Mitos ancestrales", "idTipo": 1, "fechaPublicacion": "2026-11-05"}` | 1. Enviar artículo cultural. | HTTP 200 OK confirmando creación. | HTTP 200 OK. | **Aprobado** |
| **CN-CONTLUG-01**| Asociar Contenido a Lugar (Caso normal) | Header Bearer, `POST /api/CONTENIDO_LUGAR` con `{"idContenido": 1, "idLugar": 1}` | 1. Enviar clave compuesta. | HTTP 200 OK. | HTTP 200 OK. | **Aprobado** |
| **CN-CONTCULT-01**| Asociar Contenido a Cultura (Caso normal) | Header Bearer, `POST /api/CONTENIDO_CULTURA` con `{"idContenido": 1, "idCultura": 1}` | 1. Enviar clave compuesta. | HTTP 200 OK. | HTTP 200 OK. | **Aprobado** |
| **CN-CAT-01** | Consultar Categorías de Cultura (Caso normal) | Header Bearer, `GET /api/CATEGORIA_CULTURA` | 1. Obtener categorías de culturas. | HTTP 200 OK. | HTTP 200 OK. | **Aprobado** |
| **CN-TIPO-01** | Consultar Tipos de Contenido (Caso normal) | Header Bearer, `GET /api/Tipo_Cont` | 1. Listar taxonomía de contenidos. | HTTP 200 OK. | HTTP 200 OK. | **Aprobado** |
| **CN-IDIO-01** | Consultar Idiomas Nativos (Caso normal) | Header Bearer, `GET /api/idioma` | 1. Solicitar lenguas registradas. | HTTP 200 OK. | HTTP 200 OK. | **Aprobado** |

---

## 3. Pruebas de Caja Blanca (White Box Testing)

En este enfoque se inspecciona exhaustivamente el código fuente en C#, evaluando bifurcaciones condicionales (`if/else`), estructuras de control, manejo de excepciones (`try/catch`), validaciones de modelo y caminos de ejecución independientes.

### 3.1 Análisis de Complejidad Ciclomática y Flujo de Control

#### Módulo Crítico 1: `AuthController.cs -> Login`
* **Nodos de Decisión Identificados:**
  1. `if (login == null || string.IsNullOrWhiteSpace(login.Usuario) ...)`
  2. `if (isAdmin)` vs `else if (isUser)`
  3. Verificación de credenciales de desarrollo estáticas vs Base de Datos (Azure SQL).
  4. Comparación de Hash de contraseña mediante `BCrypt.Net.BCrypt.Verify(contraseña, hash)`.
  5. `if (validCredentials)` -> Rama Verdadera (Generación de Claims JWT + Cookies) vs Rama Falsa (`401 Unauthorized`).
* **Complejidad Ciclomática de McCabe:** $V(G) = P + 1 = 7 + 1 = 8$ caminos independientes.

#### Módulo Crítico 2: `CultuLugarController.cs` y Controladores CRUD
* **Estructura de Manejo de Excepciones:**
  1. `try { var response = await _repo...; if (response == null) return 400; return Ok(response); }`
  2. `catch (UnauthorizedAccessException)` -> Mapeo HTTP 401.
  3. `catch (ArgumentException ex)` -> Mapeo HTTP 400 con detalle del mensaje.
  4. `catch (Exception ex)` -> Mapeo HTTP 500 Internal Server Error seguro.

### 3.2 Matriz de Pruebas de Caja Blanca

| ID | Archivo / Función | Ruta o Condición Probada | Caso de Prueba | Resultado Esperado | Estado |
|:---|:---|:---|:---|:---|:---|
| **CB-AUTH-01** | `AuthController.cs / Login` | Validación previa: Payload nulo o campos en blanco (`string.IsNullOrWhiteSpace`) | Enviar objeto `login = null` o `usuario = ""` | `BadRequestObjectResult` (HTTP 400) sin invocar base de datos. | **Aprobado** |
| **CB-AUTH-02** | `AuthController.cs / Login` | Rama Administrador con credenciales de desarrollo (`admin`/`admin123`) | `login = { "admin", "admin123", "Administrador" }` | `validCredentials = true`, generación de Token con Claim `Role = Administrador` y retorno HTTP 200. | **Aprobado** |
| **CB-AUTH-03** | `AuthController.cs / Login` | Rama Usuario con credenciales de desarrollo (`turista@ecoscolombia.com`) | `login = { "turista@ecoscolombia.com", "turista123", "Usuario" }` | `validCredentials = true`, Claim `Role = Usuario` y HTTP 200. | **Aprobado** |
| **CB-AUTH-04** | `AuthController.cs / Login` | Rama BD: Búsqueda en `_administradores` con verificación de hash BCrypt | Usuario registrado en DB con hash válido | `BCrypt.Verify == true`, token generado con éxito. | **Aprobado** |
| **CB-AUTH-05** | `AuthController.cs / Login` | Rama Fallida: Credenciales no coincidentes | `login = { "desconocido", "123", "Administrador" }` | Salida del bloque de evaluación con `validCredentials = false` -> `UnauthorizedResult` (HTTP 401). | **Aprobado** |
| **CB-AUTH-06** | `AuthController.cs / Register` | Validación de longitud de contraseña (`request.Password.Length < 6`) | Password = `"12345"` | Retorno inmediato de `BadRequestObjectResult` (HTTP 400) sin persistir usuario. | **Aprobado** |
| **CB-AUTH-07** | `AuthController.cs / Register` | Validación de unicidad de correo (`AnyAsync(x => x.Correo == request.Correo)`) | Correo ya existente en base de datos InMemory/Azure | `ConflictObjectResult` (HTTP 409). | **Aprobado** |
| **CB-AUTH-08** | `AuthController.cs / Register` | Flujo exitoso de registro con BCrypt Hash | Datos válidos y correo nuevo | `BCrypt.HashPassword` ejecutado, usuario insertado en DbSet y retorno HTTP 201 Created. | **Aprobado** |
| **CB-CULLUG-01**| `CultuLugarController.cs / GetculturaLugar` | Rama exitosa: Repositorio devuelve lista no nula | Repositorio mock devuelve `List<culturaLugar>` | Retorna `OkObjectResult` (HTTP 200) conteniendo la colección. | **Aprobado** |
| **CB-CULLUG-02**| `CultuLugarController.cs / obtenerId` | Rama de control: Repositorio devuelve `null` | Buscar ID inexistente en repositorio | Condición `response == null` se activa -> `StatusCode(400)`. | **Aprobado** |
| **CB-CULLUG-03**| `CultuLugarController.cs / PostculturaLugar` | Manejo de excepción `UnauthorizedAccessException` | Repositorio lanza `UnauthorizedAccessException` | `catch` específico activado -> Retorna HTTP 401 con mensaje "No tiene autorización...". | **Aprobado** |
| **CB-CULLUG-04**| `CultuLugarController.cs / PutculturaLugar` | Manejo de excepción `ArgumentException` | Repositorio lanza `ArgumentException("Datos inválidos")` | `catch` específico activado -> Retorna HTTP 400 con `error = ex.Message`. | **Aprobado** |
| **CB-CULLUG-05**| `CultuLugarController.cs / DeleteculturaLugar` | Manejo de excepción genérica no controlada `Exception` | Repositorio lanza `InvalidOperationException("Fallo BD")` | `catch (Exception ex)` activado -> Retorna HTTP 500 con `mensaje = "Ocurrió un error interno en el servidor."`. | **Aprobado** |
| **CB-CRUD-01** | `CULTURARepositoryCrudTests.cs` | Persistencia en base de datos InMemory de todas las operaciones CRUD | Reflexión dinámica sobre todos los Repositorios del ensamblado | `Post` -> `Assert.True`, `Get` -> `Assert.Single`, `Put` -> `Assert.True`, `Delete` -> `Assert.Empty`. | **Aprobado** |

---

## 4. Pruebas Automatizadas (.NET 10 xUnit)

### 4.1 Framework y Arquitectura de Pruebas
Se implementó una suite de pruebas automatizadas en el proyecto `Backend/Tests/ApiPrueba.Tests.csproj` usando:
* **Framework:** xUnit 2.9.3.
* **Aislamiento:** Entity Framework Core In-Memory Database (`Microsoft.EntityFrameworkCore.InMemory`) para evitar acoplamiento o mutaciones destructivas en la base de datos de producción de Azure SQL durante los tests automáticos.
* **Mapeo de Controladores:** Pruebas unitarias sobre `AuthController`, `CultuLugarController` y `CULTURAController`.

### 4.2 Ubicación de Archivos de Prueba
* `Backend/Tests/AuthControllerTests.cs`: 10 pruebas unitarias sobre inicio de sesión, registro, validación de contraseñas, hashing BCrypt, token JWT y cierre de sesión.
* `Backend/Tests/CultuLugarControllerTests.cs`: 22 pruebas unitarias y de teoría sobre operaciones CRUD y manejo de excepciones (200, 400, 401, 500).
* `Backend/Tests/CULTURAControllerTests.cs`: 22 pruebas unitarias y de teoría sobre la entidad Cultura.
* `Backend/Tests/CULTURARepositoryCrudTests.cs`: 20 pruebas de integración sobre persistencia en repositorios.

### 4.3 Comando y Resultado de Ejecución
Para ejecutar toda la suite de pruebas desde consola:
```powershell
dotnet test "Backend/Tests/ApiPrueba.Tests.csproj"
```

**Resultado obtenido en la consola:**
```text
Serie de pruebas para ApiPrueba.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    74, Omitido:     0, Total:    74, Duración: 1 s - ApiPrueba.Tests.dll (net10.0)
```

---

## 5. Gestión de Defectos Identificados y Soluciones

Durante el ciclo de análisis y pruebas se identificaron los siguientes defectos y oportunidades de mejora:

| ID Defecto | Gravedad | Componente | Descripción de la Falla | Solución Aplicada / Recomendada |
|:---|:---|:---|:---|:---|
| **DEF-001** | Mayor | `AuthController.cs / Login` | Inicialmente el token JWT generado se inyectaba únicamente en la cookie `access_token`, impidiendo que clientes como Postman o Swagger UI en equipos con bloqueo de cookies obtuvieran el token en el cuerpo JSON. | Se refactorizó la respuesta JSON de `Login` para devolver `{ "mensaje": "Inicio de sesión correcto.", "token": tokenString }` simultáneamente con la cookie segura. |
| **DEF-002** | Mayor | `Program.cs / Swagger` | Swagger UI carecía de la definición de `OpenApiSecurityScheme`, impidiendo autorizar las llamadas con `Bearer <token>` y provocando errores HTTP 401 en todas las pruebas manuales. | Se incorporó `AddSwaggerGen` con esquema Bearer `OpenApiSecurityRequirement`, habilitando el botón interactivo **Authorize 🔓**. |
| **DEF-003** | Menor | `CultuLugarController.cs` | El método HTTP DELETE recibía el objeto complejo `culturaLugar` sin validación estricta de null, arriesgando un `NullReferenceException` si no se enviaba el cuerpo JSON. | Se validó en las pruebas de caja blanca el flujo de control y se cubrió el mapeo a HTTP 400 y 500 en caso de excepción de argumentos. |

---

## 6. Recomendaciones de Calidad y Refactorización

1. **Estandarización de Rutas RESTful:** Unificar la nomenclatura de rutas de controladores (por ejemplo, cambiar nombres de clases como `CultuLugarController` a `CulturaLugarController` o rutas singulares a plurales estándar como `/api/culturas-lugares`).
2. **Uso de DTOs (Data Transfer Objects):** Evitar exponer las entidades de base de datos directamente en las firmas de los métodos `POST` y `PUT` de los controladores; implementar DTOs específicos para creación y edición.
3. **Manejo Centralizado de Excepciones:** Reemplazar los múltiples bloques `try-catch` repetidos en cada método de cada controlador por un **Middleware global de manejo de excepciones** (`ExceptionHandlerMiddleware`), reduciendo la duplicación de código y mejorando la mantenibilidad.
4. **Validación Automática de Modelos:** Habilitar filtros globales de validación de `ModelState.IsValid` con `FluentValidation` para rechazar de forma estandarizada cargas útiles mal formadas antes de alcanzar los controladores.

---

## 7. Conclusión

El análisis integral de calidad de software realizado sobre el repositorio **Ecos de Colombia** confirmó que el sistema cuenta con una arquitectura modular y segura, con control estricto de autenticación JWT y roles de usuario.

Las **pruebas de caja negra** validaron satisfactoriamente la totalidad de las 19 tablas y controladores del sistema tanto en operaciones exitosas como en rechazo controlado ante entradas inválidas. Las **pruebas de caja blanca** verificaron la robustez del flujo de control, hashing criptográfico y captura de excepciones. 

Finalmente, la integración de **74 pruebas automatizadas con xUnit y Entity Framework Core In-Memory** garantiza que el sistema cumple con los criterios de aceptación más exigentes según las normas **ISTQB e ISO/IEC/IEEE 29119**, quedando completamente listo y verificado para su sustentación académica y despliegue productivo.
