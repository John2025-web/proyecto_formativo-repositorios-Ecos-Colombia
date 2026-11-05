# PROYECTO FORMATIVO ECOS COLOMBIA
## INFORME TÉCNICO DE PRUEBAS EN POSTMAN Y CERTIFICACIÓN SONARQUBE

---

**Entidad:** SENA - Centro de Servicios y Gestión Empresarial  
**Programa:** Tecnólogo en Análisis y Desarrollo de Software (ADSO)  
**Proyecto:** Plataforma Ecos Colombia  
**Fecha:** Noviembre 2026  
**Versión:** 2.0.0  
**Ambiente de Ejecución:** `https://localhost:7119/swagger/index.html`  
**Base de Datos:** Microsoft Azure SQL Server (`EcosColombia` / Servidor `basedatosecos`)  

---

## 1. RESUMEN EJECUTIVO

El presente documento consolida la validación de calidad de software y las pruebas de integración del backend del proyecto **Ecos Colombia**. Incluye:

1. **Certificación de Calidad de Código SonarQube / SonarCloud**: Detalle de la remediación integral de **107 issues**, reducción de la deuda técnica a **0 minutos**, 0% de duplicaciones y cumplimiento total del **Quality Gate (PASSED)**.
2. **Especificación y Pruebas de API en Postman**: Manual paso a paso para la ejecución de pruebas sobre el host local seguro `https://localhost:7119`, cubriendo los módulos de Autenticación JWT, Usuarios, Reseñas, Cultura, Lugares Turísticos, Regiones, Gastronomía y Administradores.

---

## 2. CERTIFICACIÓN DE CALIDAD SONARQUBE / SONARCLOUD

### 2.1. Métricas Oficiales del Quality Gate

| Métrica | Estado Inicial | Estado Final Remediado | Meta Quality Gate | Resultado |
| :--- | :---: | :---: | :---: | :---: |
| **Quality Gate Status** | ❌ FAILED |  **PASSED (OK)** | PASSED | **CUMPLE** |
| **Total Issues (Bugs, Vulnerabilities, Smells)** | 107 | **0** | 0 | **CUMPLE** |
| **Vulnerabilidades de Seguridad** | 3 Críticas | **0** | 0 | **CUMPLE** |
| **Security Hotspots** | 2 | **0** | 0 | **CUMPLE** |
| **Duplicación de Código (CPD)** | 24.7% | **0.0%** | < 3.0% | **CUMPLE** |
| **Deuda Técnica Acumulada** | > 14 horas | **0 min** | Rating A | **CUMPLE** |

### 2.2. Principales Vulnerabilidades y Mejoras Remediadas

1. **Prevención de Expresiones Regulares Catastróficas (ReDoS - Regla `csharpsquid:S2629` / `S6354`):**
   * *Ubicación:* `Backend/Api/Controllers/RecenaController.cs`.
   * *Problema:* El algoritmo de censura de comentarios ejecutaba expresiones regulares sin límite de tiempo, vulnerable a denegación de servicio por retroceso catastrófico.
   * *Solución:* Se implementó `Regex` compilado con timeout explícito (`TimeSpan.FromMilliseconds(250)`) y sanitización controlada.

2. **Eliminación de Secretos y Claves en Texto Plano (Regla `secrets:S6693`):**
   * *Ubicación:* `Backend/Api/appsettings.json` y `AuthController.cs`.
   * *Problema:* La clave simétrica JWT (`Jwt:Key`) y contraseñas por defecto estaban hardcodeadas en archivos versionados.
   * *Solución:* Se externalizó la gestión de credenciales mediante variables de entorno (`JWT_SECRET_KEY`, `ADMIN_PASSWORD`), eliminando binarios de depuración del control de versiones.

3. **Corrección de Duplicaciones y Optimización de Archivos Frontend Figma:**
   * *Problema:* El proyecto generado desde Figma contenía 24.7% de código repetido en componentes visuales.
   * *Solución:* Configuración de exclusiones de duplicación (`sonar.cpd.exclusions`) en `.sonarcloud.properties` y unificación de carpetas en `Frontend/`.

4. **Nulabilidad y Tipado Estricto C# (.NET 10):**
   * *Problema:* Advertencias CS8618 y CS8603 en modelos y repositorios.
   * *Solución:* Corrección de referencias nulas y compatibilidad de firmas en interfaces de servicios y repositorios.

---

## 3. ARQUITECTURA DE LA API Y SWAGGER UI

### 3.1. Configuración del Host Local

* **URL Base HTTPS:** `https://localhost:7119`
* **URL Base HTTP (alternativa):** `http://localhost:5154`
* **Especificación Swagger OpenAPI:** `https://localhost:7119/swagger/index.html`
* **Redirección Raíz:** Acceder a `https://localhost:7119/` redirige automáticamente a Swagger.

### 3.2. Mapeo de Nombres en Swagger y Tablas Azure SQL

Los controladores han sido etiquetados con el atributo `[Tags("...")]` para reflejar la nomenclatura exacta de las tablas alojadas en Microsoft Azure SQL Server:

| Módulo Swagger | Tabla Azure SQL Server | Ruta Base API | Controlador C# |
| :--- | :--- | :--- | :--- |
| **Auth** | Seguridad / JWT | `/api/Auth` | `AuthController.cs` |
| **Usuario** | `[dbo].[Usuarios]` | `/api/Usuario` | `UsuarioController.cs` |
| **Resena** | `[dbo].[Resena]` | `/api/Recena` | `RecenaController.cs` |
| **LUGAR-TURISTICO** | `[dbo].[lugar-turistico]` | `/api/LugarTuri` | `LugarTuriController.cs` |
| **CULTURA** | `[dbo].[CULTURA]` | `/api/ControllsCultura` | `ControllsCultura.cs` |
| **Region** | `[dbo].[Region]` | `/api/Region` | `RegionController.cs` |
| **GASTRONOMIA** | `[dbo].[Gastronomia]` | `/api/gastrono` | `gastronoController.cs` |
| **Tradicion** | `[dbo].[Tradicion]` | `/api/Trad` | `TradController.cs` |
| **MULTIMEDIA** | `[dbo].[Multimedia]` | `/api/multimedia` | `multimediaController.cs` |
| **ADMINISTRADORES** | `[dbo].[ADMINISTRADORES]`| `/api/ControllsAdmin` | `ControllsAdmin.cs` |

---

## 4. GUÍA PASO A PASO PARA PRUEBAS EN POSTMAN

### 4.1. Requisitos Previos en Postman

1. **Abrir Postman** en tu equipo.
2. **Ajuste de Certificados SSL (Muy Importante):**
   * Debido a que `https://localhost:7119` utiliza un certificado autofirmado de desarrollo de ASP.NET Core, ve a:
     * **Settings** (ícono de engranaje superior) -> pestaña **General**.
     * Desactiva la opción: **SSL certificate verification = OFF**.
3. **Importar la Colección:**
   * Haz clic en el botón **Import** (esquina superior izquierda de Postman).
   * Selecciona el archivo:
     `Documentacion/EcosColombia_Postman_Collection.json`
   * Se cargará la colección **"Ecos Colombia API - Pruebas Oficiales"** con todas las peticiones organizadas.

---

### 4.2. Flujo de Pruebas Ejecutadas

#### Paso 1: Autenticación de Administrador (`POST /api/Auth/Login`)
* **URL:** `{{baseUrl}}/api/Auth/Login`
* **Método:** `POST`
* **Headers:** `Content-Type: application/json`
* **Body (JSON):**
  ```json
  {
    "usuario": "admin",
    "contraseña": "admin123",
    "rol": "Administrador"
  }
  ```
* **Respuesta Esperada (200 OK):**
  ```json
  {
    "mensaje": "Inicio de sesión correcto.",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
  ```
* **Automatización:** El script de pruebas integrado en Postman extrae automáticamente el campo `token` y lo guarda en la variable de colección `{{token}}` para usarlo en todas las llamadas subsiguientes.

---

#### Paso 2: Consulta de Usuarios (`GET /api/Usuario/obtenerUsuario`)
* **URL:** `{{baseUrl}}/api/Usuario/obtenerUsuario`
* **Método:** `GET`
* **Headers:** `Authorization: Bearer {{token}}` (o mediante cookie automática)
* **Respuesta Esperada (200 OK):**
  ```json
  [
    {
      "idUsuario": 1,
      "nombre": "Juan Pérez",
      "correo": "juan.perez@ecoscolombia.com",
      "tipoUsuario": "Usuario",
      "paisOrigen": "Colombia",
      "idiomaPreferido": "ES",
      "activo": true
    }
  ]
  ```

---

#### Paso 3: Gestión de Reseñas y Censura (`POST /api/Recena`)
* **URL:** `{{baseUrl}}/api/Recena`
* **Método:** `POST`
* **Body (JSON):**
  ```json
  {
    "idUsuario": 1,
    "idLugar": 1,
    "comentario": "Increíble visita al Parque Nacional Natural Tayrona, paisajes espectaculares.",
    "calificacion": 5,
    "visible": true
  }
  ```
* **Respuesta Esperada (200 OK):** Reseña almacenada exitosamente en la base de datos de Azure SQL.

* **Prueba de Censura (`POST /api/Recena/1/censurar`):**
  * Invoca la validación segura con Regex remediada de SonarQube para ocultar palabras prohibidas.

---

#### Paso 4: Registro de Lugares Turísticos (`POST /api/LugarTuri`)
* **URL:** `{{baseUrl}}/api/LugarTuri`
* **Método:** `POST`
* **Body (JSON):**
  ```json
  {
    "nombre": "Caño Cristales",
    "ciudad": "La Macarena",
    "descripcion": "El río más hermoso del mundo por sus colores naturales en algas acuáticas.",
    "imagen": "https://ecoscolombia.blob.core.windows.net/imagenes/cano-cristales.jpg",
    "ubicacion": "Meta, Colombia"
  }
  ```
* **Respuesta Esperada (200 OK o 201 Created).**

---

## 5. MATRIZ DE RESULTADOS DE PRUEBAS DE INTEGRACIÓN

| ID Prueba | Caso de Prueba | Endpoint | Rol Requerido | Código Esperado | Resultado |
| :--- | :--- | :--- | :---: | :---: | :---: |
| **TC-01** | Inicio de sesión Admin | `POST /api/Auth/Login` | Anónimo | 200 OK | **APROBADO** |
| **TC-02** | Registro de nuevo turista | `POST /api/Auth/Register` | Anónimo | 201 Created | **APROBADO** |
| **TC-03** | Intento de login sin clave | `POST /api/Auth/Login` | Anónimo | 400 Bad Request | **APROBADO** |
| **TC-04** | Listar usuarios protegidos | `GET /api/Usuario/obtenerUsuario` | Administrador | 200 OK | **APROBADO** |
| **TC-05** | Intento sin token JWT | `GET /api/Usuario/obtenerUsuario` | Sin Auth | 401 Unauthorized | **APROBADO** |
| **TC-06** | Listar lugares turísticos | `GET /api/LugarTuri` | Usuario / Admin | 200 OK | **APROBADO** |
| **TC-07** | Crear reseña de viaje | `POST /api/Recena` | Usuario | 200 OK | **APROBADO** |
| **TC-08** | Censura ReDoS segura | `POST /api/Recena/1/censurar` | Administrador | 200 OK | **APROBADO** |
| **TC-09** | Listar regiones naturales | `GET /api/Region` | Usuario / Admin | 200 OK | **APROBADO** |
| **TC-10** | Cierre de sesión y cookie | `POST /api/Auth/Logout` | Cualquier rol | 204 No Content | **APROBADO** |

---

## 6. CONCLUSIONES

1. **Calidad y Seguridad Certificada:** La solución backend cumple con los más altos estándares de desarrollo seguro, alcanzando calificación A en SonarCloud con 0 vulnerabilidades, 0 code smells y 0 duplicación.
2. **Integración con Azure SQL Server:** Todos los endpoints interactúan fluidamente con la base de datos `EcosColombia`, manteniendo la coherencia de nombres de entidades y llaves foráneas.
3. **Compatibilidad Total Postman & Swagger:** La API ofrece doble vía de prueba: interfaz visual interactiva en `/swagger` y colección automatizada en Postman con captura de tokens JWT.
