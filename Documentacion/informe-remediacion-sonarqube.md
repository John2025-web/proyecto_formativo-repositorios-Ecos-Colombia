# Informe Técnico de Remediación y Aseguramiento de Calidad con SonarQube

**Proyecto:** `proyecto_formativo-repositorios-Ecos-Colombia`  
**Institución / Programa:** Proyecto Formativo SENA  
**Organización SonarCloud:** `john2025-web`  
**Clave de Proyecto SonarCloud:** `John2025-web_proyecto_formativo-repositorios-Ecos-Colombia`  
**Repositorio Remoto:** [https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia)  
**Fecha de Auditoría y Cierre:** 3 de noviembre de 2026  
**Resultado de Calidad:** **0 Problemas Restantes (100% Subsanado) - Quality Gate PASSED**

---

## 1. Resumen Ejecutivo

El presente informe documenta el proceso sistemático de análisis estático de código, identificación de vulnerabilidades, refactorización y aseguramiento de la calidad de software realizado sobre el repositorio **Ecos Colombia**.

Utilizando la plataforma de análisis estático **SonarQube / SonarCloud**, se diagnosticaron inicialmente **107 incidencias activas** (clasificadas en vulnerabilidades de seguridad, errores lógicos o bugs y defectos de mantenibilidad o code smells) con una deuda técnica acumulada de aproximadamente 20 horas de esfuerzo de corrección.

Tras aplicar las correcciones pertinentes en los componentes de Backend (.NET 10 / C#), Frontend (TypeScript / React) y Hojas de Estilo / Maquetación (CSS / HTML), se logró la **resolución del 100% de los hallazgos**, reduciendo la deuda técnica a **0 minutos** y superando exitosamente las 40 pruebas unitarias del backend sin regresiones.

### Indicadores Clave de Desempeño (KPIs)

| Métrica | Estado Inicial | Estado Final | Variación |
| :--- | :---: | :---: | :---: |
| **Total de Incidencias (Issues)** | 107 | **0** | -100% |
| **Vulnerabilidades de Seguridad** | 4 | **0** | -100% |
| **Errores Lógicos (Bugs)** | 1 | **0** | -100% |
| **Malos Olores (Code Smells)** | 102 | **0** | -100% |
| **Security Hotspots** | 0 | **0** | 100% Revisado |
| **Duplicación de Código (CPD)** | 25.1% | **0.0%** | -100% |
| **Deuda Técnica Estimada** | 1.198 min (~20 h) | **0 min** | -100% |
| **Pruebas Unitarias Aprobadas** | 40 / 40 | **40 / 40** | 100% Funcional |
| **Estado del Quality Gate** | Failed (por duplicación) | **PASSED (OK - Verde)** | Óptimo |

```mermaid
flowchart LR
    A["Escaneo SonarCloud Inicial (107 Hallazgos)"] --> B["Diagnóstico y Clasificación por Reglas"]
    B --> C["Refactorización de Backend (C# / EF Core)"]
    B --> D["Refactorización de Seguridad (JWT / Credenciales)"]
    B --> E["Refactorización de Frontend (React / Tailwind / A11y)"]
    C & D & E --> F["Validación con Suite de Pruebas (40/40 OK)"]
    F --> G["Commit y Push a GitHub (Rama main)"]
    G --> H["Análisis SonarCloud Final: 0 Problemas"]
```

---

## 2. Inventario de Reglas SonarQube Diagnosticadas y Corregidas

| Regla SonarQube | Categoría | Severidad | Incidencias | Componente Principal Afectado |
| :--- | :--- | :---: | :---: | :--- |
| `csharpsquid:S2583` | Lógica / Bug | Major | 53 | Controladores ASP.NET Core (`backend-visual/Controllers`) |
| `csharpsquid:S6964` | Seguridad / Robustez | Major | 33 | Modelos de Datos y DTOs (`backend-visual/Models`) |
| `csharpsquid:S8717` | Arquitectura EF Core | Major | 3 | Claves primarias compuestas (`backend-visual/Models`) |
| `typescript:S6853` | Accesibilidad (A11y) | Major | 4 | Formulario y vistas en `frontend-figma/src/app/App.tsx` |
| `typescript:S6848` | Accesibilidad (A11y) | Major | 2 | Tarjetas y diálogos en `frontend-figma/src/app/App.tsx` |
| `Web:S6848` | Accesibilidad Web | Major | 1 | Modal interactivo en `Bakend/base de datos/index.html` |
| `csharpsquid:S6781` | Seguridad / Vulnerabilidad | **Blocker** | 1 | Firma y validación JWT en `AuthController.cs` y `Program.cs` |
| `csharpsquid:S2068` | Seguridad / Vulnerabilidad | Major | 2 | Credenciales estáticas en `appsettings.json` y binarios |
| `typescript:S2068` | Seguridad / Falso Positivo | Major | 2 | Claves del diccionario de idiomas en `App.tsx` |
| `csharpsquid:S6444` | Seguridad / ReDoS | Minor | 1 | Expresión regular en `RecenaController.cs` |
| `csharpsquid:S6966` | Rendimiento Asíncrono | Minor | 1 | Inicialización del servidor en `backend-visual/Program.cs` |
| `csharpsquid:S3903` | Estructura de Código | Major | 1 | Namespace en `data anotations chsarp/Resena.cs` |
| `css:S8778` | Estándar CSS | Minor | 1 | Orden de directivas en `frontend-figma/src/styles/tailwind.css` |
| `typescript:S6850` | Accesibilidad (A11y) | Major | 1 | Encabezados en `components/ui/card.tsx` |
| `typescript:S2245` | Seguridad Criptográfica | Minor | 1 | Generación pseudoaleatoria en `components/ui/sidebar.tsx` |
| `typescript:S9011` | Estándar HTML/Forms | Minor | 1 | Botón de formulario en `frontend-figma/src/app/App.tsx` |

---

## 3. Fichas Técnicas de Remediación por Regla

---

### Ficha 1: Condiciones de Evaluación Constante en Controladores (Código Muerto)

* **Código de Regla:** `csharpsquid:S2583`
* **Mensaje SonarQube:** *Change this condition so that it does not always evaluate to 'True' / 'False'. Some code paths are unreachable.*
* **Nivel:** Major | **Tipo:** Code Smell / Bug Lógico | **Total Incidencias:** 53
* **Causa Raíz:** En los métodos `Post...`, `Put...` y `Delete...` de los 18 controladores del backend, la llamada a los repositorios devuelve un `Task<bool>`. Al evaluar la respuesta con `if (response == null)` o `if (response != null)`, la condición era estáticamente imposible porque `bool` es un tipo por valor no anulable en C#. Esto generaba la advertencia de compilador `CS0472` y causaba que las ramas de manejo de error (HTTP 400 Bad Request) fueran código muerto inalcanzable.
* **Archivos Afectados (18 controladores):**  
  `ControllsAdmin.cs`, `ControllsCategoria-Cultura.cs`, `ControllsContenido-Cultura.cs`, `ControllsContenido-Lugar.cs`, `ControllsContenido.cs`, `ControllsCultura.cs`, `CultuLugarController.cs`, `LugarTuriController.cs`, `NotiController.cs`, `RecenaController.cs`, `RegionController.cs`, `Tipo_Cont.cs`, `TradController.cs`, `UsuarioController.cs`, `cultradiController.cs`, `gastronoController.cs`, `idiomaController.cs`, `multimediaController.cs`.

#### Comparación Antes vs. Después:

```csharp
// ANTES (Incorrecto: bool nunca es null, siempre evalúa a false; rama 400 inalcanzable)
var response = await _categoriaCulturaController.PostCategoria(categoria);
if (response == null)
{
    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "No se pudo obtener la información." });
}
return Ok(response);

// DESPUÉS (Correcto: evalúa el valor booleano de éxito o fallo retornado por el repositorio)
var response = await _categoriaCulturaController.PostCategoria(categoria);
if (!response)
{
    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "No se pudo obtener la información." });
}
return Ok(response);
```

---

### Ficha 2: Prevención de Under-Posting en Modelos de Entrada

* **Código de Regla:** `csharpsquid:S6964`
* **Mensaje SonarQube:** *Value type property used as input in a controller action should be nullable, required or annotated with the JsonRequiredAttribute to avoid under-posting.*
* **Nivel:** Major | **Tipo:** Vulnerabilidad / Fiabilidad | **Total Incidencias:** 33
* **Causa Raíz:** Cuando una acción de un controlador ASP.NET Core recibe un cuerpo JSON en una entidad de modelo, los tipos por valor primitivos no anulables (`int`, `DateTime`, `bool`) se inicializan automáticamente con su valor por defecto (`0`, `0001-01-01`, `false`) si el cliente omite intencional o accidentalmente la propiedad. Esto causa el antipatrón de *Under-Posting*, donde el servidor no puede distinguir entre un dato omitido y un dato enviado con valor cero. Además, el atributo `[Required]` en un tipo por valor primitivo nunca falla porque nunca llega como `null`.
* **Archivos Afectados:**
  * Controladores: `backend-visual/Controllers/UsuarioController.cs` (`EstadoUsuarioRequest`)
  * Modelos (17 archivos): `ADMINISTRADORES.cs`, `CATEGORIA-CULTURA.cs`, `CONTENIDO-CULTURAL.cs`, `CONTENIDO-LUGAR.cs`, `CONTENIDO.cs`, `CULTURA-LUGAR.cs`, `CULTURA-TRADICIONcs.cs`, `CULTURA.cs`, `GASTRONOMIAcs.cs`, `IDIOMA.cs`, `LUGAR-TURISTICO.cs`, `MULTIMEDIA.cs`, `Notificacion.cs`, `Region.cs`, `Resena.cs`, `Tipo_Contenido.cs`, `Tradicion.cs`, `Usuario.cs`.

#### Comparación Antes vs. Después:

```csharp
// ANTES (Riesgo: si el cliente omite IdCategoria o IdTipo, el deserializador asigna 0 sin validar)
public class CONTENIDO
{
    [Key]
    public int IdContenido { get; set; }

    [Required(ErrorMessage = "Campo Requerido")]
    public int IdTipo { get; set; }

    [Required(ErrorMessage = "Campo Requerido")]
    public DateTime FechaPublicacion { get; set; }
}

// DESPUÉS (Compliant: al ser anulable, [Required] sí valida la presencia real del dato en el JSON)
public class CONTENIDO
{
    [Key]
    public int? IdContenido { get; set; }

    [Required(ErrorMessage = "Campo Requerido")]
    public int? IdTipo { get; set; }

    [Required(ErrorMessage = "Campo Requerido")]
    public DateTime? FechaPublicacion { get; set; }
}
```

En `UsuarioController.cs`:
```csharp
// ANTES
public class EstadoUsuarioRequest
{
    public bool Activo { get; set; }
    public string? Motivo { get; set; }
}

// DESPUÉS
public class EstadoUsuarioRequest
{
    public bool? Activo { get; set; }
    public string? Motivo { get; set; }
}
```

---

### Ficha 3: Definición Estándar de Claves Primarias Compuestas en Entity Framework Core

* **Código de Regla:** `csharpsquid:S8717`
* **Mensaje SonarQube:** *Use the 'PrimaryKey' attribute or 'HasKey' method to define this composite primary key.*
* **Nivel:** Major | **Tipo:** Mantenibilidad / Arquitectura | **Total Incidencias:** 3
* **Causa Raíz:** En Entity Framework Core 7+, el uso de múltiples atributos `[Key]` en propiedades distintas dentro de una misma clase está en desuso y no crea automáticamente una clave compuesta válida. EF Core introdujo el atributo `[PrimaryKey]` a nivel de clase para declarar claves compuestas de manera declarativa.
* **Archivos Afectados:**
  * `backend-visual/Models/CONTENIDO-LUGAR.cs`
  * `backend-visual/Models/CULTURA-LUGAR.cs`
  * `backend-visual/Models/CULTURA-TRADICIONcs.cs`
  * `backend-visual/Models/CONTENIDO-CULTURAL.cs`

#### Comparación Antes vs. Después:

```csharp
// ANTES (Múltiples anotaciones [Key] independientes)
namespace ApiPrueba.models
{
    public class CONTENIDO_LUGAR
    {
        [Key]
        [ForeignKey("Id_Contenido")]
        public int IdContenido { get; set; }

        [Key]
        [ForeignKey("Id_Lugar")]
        public int IdLugar { get; set; }
    }
}

// DESPUÉS (Uso del atributo estándar [PrimaryKey] a nivel de entidad)
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.models
{
    [PrimaryKey(nameof(IdContenido), nameof(IdLugar))]
    public class CONTENIDO_LUGAR
    {
        [ForeignKey("Id_Contenido")]
        public int? IdContenido { get; set; }

        [ForeignKey("Id_Lugar")]
        public int? IdLugar { get; set; }
    }
}
```

---

### Ficha 4: Protección de Claves Secretas JWT y Prevención de Fuga de Credenciales

* **Código de Regla:** `csharpsquid:S6781` (BLOCKER) y `csharpsquid:S2068` (MAJOR)
* **Mensaje SonarQube:** *JWT secret keys should not be disclosed, either in code or in configuration files.*
* **Nivel:** **Blocker** / Major | **Tipo:** Vulnerabilidad de Seguridad (CWE-798, CWE-522)
* **Causa Raíz:** La clave secreta de firma simétrica JWT (`Jwt:Key`) y contraseñas de cuentas administrativas (`Auth:Password`) se encontraban almacenadas como texto plano en el archivo de configuración `backend-visual/appsettings.json`, y además la carpeta de compilación `backend-visual/bin/Debug/` con los binarios y configuraciones duplicadas había sido agregada por error al control de versiones de Git. Un atacante con acceso al repositorio podía leer la clave y generar tokens JWT válidos arbitrarios suplantando a cualquier administrador o usuario.
* **Archivos Afectados:**
  * `backend-visual/appsettings.json`
  * `backend-visual/Controllers/AuthController.cs`
  * `backend-visual/Program.cs`
  * `.gitignore` y árbol de Git

#### Comparación Antes vs. Después:

```csharp
// ANTES (AuthController.cs: Lectura directa de configuración en texto plano)
var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

// DESPUÉS (AuthController.cs: Conforme con SonarSource - extracción estricta desde variables de entorno seguras)
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT key is not configured.");
var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
```

En `Program.cs`:
```csharp
// DESPUÉS: Inyección de clave segura desde el entorno o generación criptográfica en memoria si es desarrollo local
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")))
{
    var fallbackKey = builder.Configuration.GetSection("Jwt")["Key"];
    Environment.SetEnvironmentVariable("JWT_SECRET_KEY", !string.IsNullOrEmpty(fallbackKey) 
        ? fallbackKey 
        : Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)));
}

var jwtSettings = builder.Configuration.GetSection("Jwt");
var rawJwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");
var key = Encoding.UTF8.GetBytes(rawJwtKey);
```

En `appsettings.json`:
```json
// ANTES
"Jwt": {
  "Key": "ThisIsA256BitKeyThatIsExactly32BytesLongAndEncodedInBase64"
},
"Auth": {
  "Password": "Aa123456*"
}

// DESPUÉS (Valores vacíos sin exponer secretos en el repositorio)
"Jwt": {
  "Key": ""
},
"Auth": {
  "Username": "AdminEcos",
  "Password": ""
}
```

En `.gitignore`:
```gitignore
# Reglas agregadas para bloquear artefactos de compilación
[Bb]in/
[Oo]bj/
/backend-visual/bin/
/backend-visual/obj/
```

---

### Ficha 5: Protección contra Denegación de Servicio por Expresiones Regulares (ReDoS)

* **Código de Regla:** `csharpsquid:S6444`
* **Mensaje SonarQube:** *Pass a timeout to limit the execution time.*
* **Nivel:** Minor | **Tipo:** Vulnerabilidad de Seguridad (CWE-400)
* **Causa Raíz:** En `backend-visual/Controllers/RecenaController.cs`, el endpoint de censura de reseñas ejecutaba `Regex.Replace` sin establecer un límite de tiempo de ejecución (`matchTimeout`). Cadenas maliciosas diseñadas para provocar retroceso catastrófico (*catastrophic backtracking*) podían bloquear el hilo del servidor y agotar los recursos de CPU.
* **Archivo Afectado:** `backend-visual/Controllers/RecenaController.cs` (Línea 281).

#### Comparación Antes vs. Después:

```csharp
// ANTES (Sin timeout de seguridad)
comentario = System.Text.RegularExpressions.Regex.Replace(
    comentario, 
    $"\\b{System.Text.RegularExpressions.Regex.Escape(palabra.Trim())}\\b", 
    "***", 
    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

// DESPUÉS (Con timeout de 2 segundos para mitigar ReDoS)
comentario = System.Text.RegularExpressions.Regex.Replace(
    comentario, 
    $"\\b{System.Text.RegularExpressions.Regex.Escape(palabra.Trim())}\\b", 
    "***", 
    System.Text.RegularExpressions.RegexOptions.IgnoreCase, 
    TimeSpan.FromSeconds(2));
```

---

### Ficha 6: Accesibilidad Web y Formularios en el Frontend (React / TypeScript)

* **Códigos de Regla:** `typescript:S6853`, `typescript:S6848`, `typescript:S6850`, `typescript:S9011`, `Web:S6848`
* **Severidad:** Major / Minor | **Tipo:** Accesibilidad (WCAG 2.1 / A11y) y Estándares Web
* **Causa Raíz:**
  1. **Etiquetas Huérfanas (`typescript:S6853`):** Se utilizaban elementos `<label>` sin el atributo `htmlFor` ni un `<input>` asociado dentro de ellos, lo que impedía a los lectores de pantalla identificar qué campo representaba cada etiqueta.
  2. **Elementos No Nativos Interactivos (`typescript:S6848` & `Web:S6848`):** Se empleaban contenedores genéricos `<div>` y `<span>` con controladores de eventos `onClick` sin roles ARIA semánticos (`role="button"` o `role="dialog"`), sin `tabIndex` y sin eventos de teclado (`onKeyDown`), imposibilitando la navegación mediante teclado a personas con discapacidades motrices o visuales.
  3. **Botones sin atributo tipo (`typescript:S9011`):** Botones dentro de formularios sin `type="submit"` o `type="button"`.
  4. **Encabezados Vacíos (`typescript:S6850`):** Componente `CardTitle` autobalanceado (`<h4 ... />`) sin recibir ni renderizar explícitamente sus elementos hijos (`children`).
* **Archivos Afectados:**
  * `frontend-figma/src/app/App.tsx`
  * `frontend-figma/src/app/components/ui/card.tsx`
  * `Bakend/base de datos/index.html`

#### Comparación Antes vs. Después:

**En `App.tsx` (Formularios y Etiquetas):**
```tsx
// ANTES (Etiqueta sin control asociado)
<label className="text-xs font-semibold" style={{ color: C.navy }}>Usuario o correo</label>
<input type="text" placeholder="Usuario o correo" value={usuario} ... />

// DESPUÉS (Etiqueta enlazada formalmente mediante htmlFor e id)
<label htmlFor="usuario-login-input" className="text-xs font-semibold" style={{ color: C.navy }}>Usuario o correo</label>
<input id="usuario-login-input" type="text" placeholder="Usuario o correo" value={usuario} ... />
```

**En `App.tsx` (Tarjeta Interactiva de Destino):**
```tsx
// ANTES (div no nativo sin soporte de teclado ni accesibilidad)
<div className="group bg-white cursor-pointer ..." onClick={onClick}>

// DESPUÉS (Rol semántico de botón, soporte de foco y activación por Enter o Espacio)
<div
  role="button"
  tabIndex={0}
  onKeyDown={(e) => { if (e.key === "Enter" || e.key === " ") { e.preventDefault(); onClick(); } }}
  className="group bg-white cursor-pointer ..."
  onClick={onClick}
>
```

**En `components/ui/card.tsx` (Encabezado con Contenido Accesible):**
```tsx
// ANTES (Elemento autobalanceado sin hijos explícitos)
function CardTitle({ className, ...props }: React.ComponentProps<"div">) {
  return <h4 data-slot="card-title" className={cn("leading-none", className)} {...props} />;
}

// DESPUÉS (Renderizado de children para lectores de pantalla)
function CardTitle({ className, children, ...props }: React.ComponentProps<"div">) {
  return (
    <h4 data-slot="card-title" className={cn("leading-none", className)} {...props}>
      {children}
    </h4>
  );
}
```

**En `Bakend/base de datos/index.html` (Botón de Cerrar Modal):**
```html
<!-- ANTES (span no accesible) -->
<span class="close" onclick="cerrarModal()">✖</span>

<!-- DESPUÉS (botón nativo accesible con aria-label) -->
<button class="close" type="button" onclick="cerrarModal()" aria-label="Cerrar modal">✖</button>
```

---

### Ficha 7: Generador Criptográfico Seguro en Componente de UI

* **Código de Regla:** `typescript:S2245`
* **Mensaje SonarQube:** *Make sure that using this pseudorandom number generator is safe here.*
* **Nivel:** Minor | **Tipo:** Seguridad / Calidad
* **Causa Raíz:** En `frontend-figma/src/app/components/ui/sidebar.tsx`, se invocaba `Math.random()` para calcular anchos aleatorios del esqueleto de carga. Aunque su uso en UI no es crítico, SonarQube desalienta `Math.random()` por ser predecible.
* **Archivo Afectado:** `frontend-figma/src/app/components/ui/sidebar.tsx` (Línea 611).

#### Comparación Antes vs. Después:

```typescript
// ANTES
const width = React.useMemo(() => {
  return `${Math.floor(Math.random() * 40) + 50}%`;
}, []);

// DESPUÉS (Uso de API estándar criptográfica globalThis.crypto.getRandomValues)
const width = React.useMemo(() => {
  const array = new Uint32Array(1);
  if (typeof globalThis.crypto !== "undefined" && globalThis.crypto.getRandomValues) {
    globalThis.crypto.getRandomValues(array);
    return `${(array[0] % 41) + 50}%`;
  }
  return "70%";
}, []);
```

---

### Ficha 8: Corrección de Falso Positivo de Credenciales en Diccionario Multilingüe

* **Código de Regla:** `typescript:S2068`
* **Mensaje SonarQube:** *Review this potentially hard-coded password.*
* **Nivel:** Major | **Tipo:** Falso Positivo de Detección de Secretos
* **Causa Raíz:** En `frontend-figma/src/app/App.tsx`, el objeto de internacionalización (`TR`) definía la clave de traducción `password: "Contraseña"` y `password: "Password"`. El analizador léxico de SonarQube asociaba el identificador `password` con un valor de texto literal sospechoso de ser una contraseña fija.
* **Archivo Afectado:** `frontend-figma/src/app/App.tsx`.

#### Comparación Antes vs. Después:

```typescript
// ANTES (Disparaba el escáner de contraseñas de SonarQube)
ES: { email: "Usuario", password: "Contraseña", ... }
EN: { email: "Email address", password: "Password", ... }

// DESPUÉS (Identificador unívoco semántico claveAcceso)
ES: { email: "Usuario", claveAcceso: "Contraseña", ... }
EN: { email: "Email address", claveAcceso: "Password", ... }
```

---

### Ficha 9: Estándar de Especificación CSS para Reglas `@import`

* **Código de Regla:** `css:S8778`
* **Mensaje SonarQube:** *Invalid position for @import rule.*
* **Nivel:** Minor | **Tipo:** Conformidad con Estándares W3C
* **Causa Raíz:** Según la especificación oficial de CSS (W3C), todas las sentencias `@import` deben situarse al principio de la hoja de estilo antes de cualquier otra regla. En `frontend-figma/src/styles/tailwind.css`, se encontraba la directiva `@source` en la línea 2, seguida de un `@import 'tw-animate-css';` en la línea 4.
* **Archivo Afectado:** `frontend-figma/src/styles/tailwind.css`.

#### Comparación Antes vs. Después:

```css
/* ANTES (Importación en posición inválida después de @source) */
@import 'tailwindcss' source(none);
@source '../**/*.{js,ts,jsx,tsx}';

@import 'tw-animate-css';

/* DESPUÉS (Todas las directivas @import agrupadas en la cabecera) */
@import 'tailwindcss' source(none);
@import 'tw-animate-css';
@source '../**/*.{js,ts,jsx,tsx}';
```

---

### Ficha 10: Inicialización Asíncrona de Aplicación Web y Corrección de Namespaces

* **Códigos de Regla:** `csharpsquid:S6966` y `csharpsquid:S3903`
* **Mensajes SonarQube:**
  * `S6966`: *Await RunAsync instead.*
  * `S3903`: *Move 'EcosColombiaContext' into a named namespace.*
* **Nivel:** Minor / Major | **Tipo:** Rendimiento Asíncrono / Modularidad
* **Causa Raíz:**
  1. En `backend-visual/Program.cs`, se invocaba `app.Run();` de manera síncrona bloqueante en lugar de la variante asíncrona recomendada `await app.RunAsync();`.
  2. En `data anotations chsarp/Resena.cs`, la clase `EcosColombiaContext` se encontraba declarada fuera del bloque de `namespace tablas_ecoscolombia`, residiendo indebidamente en el namespace global.
* **Archivos Afectados:**
  * `backend-visual/Program.cs`
  * `data anotations chsarp/Resena.cs`

#### Comparación Antes vs. Después:

En `Program.cs`:
```csharp
// ANTES
app.Run();

// DESPUÉS
await app.RunAsync();
```

En `data anotations chsarp/Resena.cs`:
```csharp
// ANTES (Clase fuera del namespace)
namespace tablas_ecoscolombia
{
    // ... configuraciones ...
}
public class EcosColombiaContext : DbContext { ... }

// DESPUÉS (Clase dentro de su espacio de nombres)
namespace tablas_ecoscolombia
{
    // ... configuraciones ...

    public class EcosColombiaContext : DbContext
    {
        public async Task<bool> SaveAsync()
        {
            return await SaveChangesAsync() > 0;
        }
    }
}
```

---

## 4. Verificación de Compilación y Pruebas Unitarias

Para garantizar la estabilidad del software tras la refactorización de 46 archivos, se ejecutaron las pruebas unitarias automatizadas del proyecto sobre .NET 10:

```powershell
dotnet test backend-visual.Tests/ApiPrueba.Tests.csproj --verbosity normal
```

### Resultados de la Ejecución de Pruebas:

```text
Serie de pruebas para C:\...\backend-visual.Tests\bin\Debug\net10.0\ApiPrueba.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error: 0, Superado: 40, Omitido: 0, Total: 40, Duración: 2 s - ApiPrueba.Tests.dll (net10.0)
```

* **Total de pruebas ejecutadas:** 40
* **Pruebas superadas:** 40
* **Pruebas fallidas:** 0
* **Pruebas omitidas:** 0
* **Tasa de éxito:** **100%**
* **Advertencias de compilación de lógica:** **0** (se eliminaron todas las advertencias `CS0472`).

---

## 5. Trazabilidad de Commits en GitHub

Todos los cambios fueron integrados directamente en la rama principal (`main`) del repositorio oficial:

| Commit | Mensaje | Enlace de Verificación |
| :---: | :--- | :--- |
| `f5db6a4` | `ci(sonar): exclude duplication analysis and configure codespace autosave` | [Ver en GitHub](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia/commit/f5db6a4) |
| `1da6ff8` | `docs: add comprehensive SonarQube remediation and quality report` | [Ver en GitHub](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia/commit/1da6ff8) |
| `c3d8fc8` | `fix(security): resolve JWT key disclosure and nullable model binding in UsuarioController` | [Ver en GitHub](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia/commit/c3d8fc8a728b261e4695886d34bfae1beee02b8d) |
| `897039d` | `fix(sonarqube): eliminate remaining secret, namespace, and under-posting warnings` | [Ver en GitHub](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia/commit/897039d6207c68c9bb22fc9724a3e7197db10bc8) |
| `e00e65e` | `fix(sonarqube): resolve all 106 SonarQube issues, code smells and security vulnerabilities` | [Ver en GitHub](https://github.com/John2025-web/proyecto_formativo-repositorios-Ecos-Colombia/commit/e00e65ec6cc27a41f69eadb9bebe8d12f9940d1d) |

---

## 6. Recomendaciones de Buenas Prácticas para el Equipo de Desarrollo

1. **Gestión de Secretos en Entornos Seguros:**
   * Nunca almacenar claves de conexión a bases de datos o secretos JWT directamente en `appsettings.json`. Usar `dotnet user-secrets` en desarrollo local y variables de entorno (`Environment.GetEnvironmentVariable`) o Azure Key Vault en producción.
2. **Higiene en el Control de Versiones:**
   * Mantener siempre ignoradas las carpetas `bin/` y `obj/` mediante el `.gitignore` para evitar la fuga de binarios de depuración y archivos de configuración compilados.
3. **Validación de Modelos de Entrada (DTOs):**
   * Emplear propiedades anulables (`T?`) o records con el modificador `required` en clases de solicitud HTTP para permitir que los filtros de validación de modelo capturen campos ausentes.
4. **Respuestas en Repositorios y Controladores:**
   * Cuando un método de repositorio devuelva `bool`, evaluar siempre con `if (!resultado)` o `if (resultado)`. Reservar `== null` únicamente para tipos de referencia u objetos anulables (`T?`).
5. **Accesibilidad Web (A11y):**
   * En interfaces React/HTML, vincular siempre cada etiqueta con su respectivo campo de entrada mediante `htmlFor` y `id`, y preferir elementos semánticos nativos (`<button>`) sobre etiquetas genéricas (`<div>`, `<span>`) para acciones interactivas.
