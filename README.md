# BiblioTech


## Tecnologías

- .NET 10 (SDK 10.0.x)
- ASP.NET Core Web API + Scalar
- Blazor Web App (WebAssembly)
- Entity Framework Core 10 (Database First)
- SQL Server

## Estructura

```
BiblioTech2/
├── database/                  → BiblioTech.sql (local) y BiblioTech.Azure.sql
├── BiblioTech2.Api/           → API REST (la única que toca la BD)
└── BiblioTech2.Web/
    ├── BiblioTech2.Web/        → servidor: entrega la app al navegador
    └── BiblioTech2.Web.Client/ → la app que corre en el navegador
```

“El Proyecto .Web arma una caja con nuestra app en C#, ya compilada, y se la envía al navegador. El navegador la abre y ejecuta el C# ahí mismo, 
en el PC del usuario, sin instalar nada, porque el navegador ya sabe correr WebAssembly.
La caja corre en una zona segura y, cuando necesita datos, se los pide a la API.  
Queda guardada en el cache del navegador asi la segunda vez no se tiene que descargar de nuevo.”

## Requisitos

- Visual Studio 2026 con la carga "ASP.NET y desarrollo web"
- SDK .NET 10 → verificar con: `dotnet --list-sdks`
- SQL Server + SSMS

## Cómo ejecutarlo (primera vez)

1. Hacer fork y clonar el repositorio.
2. Abrir `database/BiblioTech.sql` en SSMS y ejecutarlo (F5).
   Crea la BD `BiblioTechManual` con tablas, vista, procedimiento y datos.
3. Abrir `BiblioTech2.slnx` en Visual Studio.
4. **Configurar TU cadena de conexión** (cada PC tiene un servidor distinto):
   clic derecho en `BiblioTech2.Api` → **Administrar secretos de usuario** y pegar:
   ```json
   {
     "ConnectionStrings": {
       "BiblioTechConnection": "Data Source=localhost;Initial Catalog=BiblioTechManual;Integrated Security=True;Trust Server Certificate=True"
     }
   }
   ```
   Cambia `localhost` por tu servidor (ej: `localhost\SQLEXPRESS`). Los secretos ganan sobre `appsettings.json` y **no se suben a GitHub**.
5. Revisar los puertos (ver sección más abajo).
6. Elegir el perfil **Api + Web** arriba junto al ▶ y ejecutar (F5).
   - La API abre Scalar.
   - La Web abre la página de Blazor.
7. **Crear tu usuario Admin** (solo la primera vez): en Scalar o en `BiblioTech2.Api.http`,
   ejecutar `POST /api/auth/registro` con `{ "email": "admin@biblio.com", "password": "Admin123" }`.
   El primer usuario queda Admin; luego `POST /api/auth/login` devuelve el token.

## La API

Todas las respuestas usan el mismo sobre: `{ "exito", "mensaje", "datos", "errores" }`.
**Leer es libre; escribir (POST/PUT/PATCH/DELETE) requiere token** → `Authorization: Bearer {token}`.

| Módulo | Endpoints |
|---|---|
| Auth | `POST /api/auth/login` · `POST /api/auth/registro` · `GET /api/auth/yo` 🔒 |
| Autores | `GET /api/autores` · `GET /{id}` · `POST` 🔒 · `PUT /{id}` 🔒 · `DELETE /{id}` 🔒 |
| Libros | `GET /api/libros?buscar=` · `GET /{id}` · `POST` 🔒 · `PUT /{id}` 🔒 · `DELETE /{id}` 🔒 |
| Socios | `GET /api/socios?activos=` · `GET /{id}` · `POST` 🔒 · `PUT /{id}` 🔒 · `PATCH /{id}/estado` 🔒 |
| Préstamos | `GET /api/prestamos?estado=&socioId=` · `GET /{id}` · `GET /vencidos` · `POST` 🔒 · `PUT /{id}/devolver` 🔒 |
| Dashboard | `GET /api/dashboard` |

| Código | Cuándo |
|---|---|
| 200 / 201 | Todo bien |
| 400 | Datos inválidos (viene `errores` por campo) |
| 401 | Sin token, token vencido o login fallido |
| 403 | Con token, pero sin el rol (ej: un Bibliotecario registrando usuarios) |
| 404 | No existe |
| 409 | Regla de negocio (ej: autor con libros, socio con 3 préstamos) |

### Reglas de préstamos

| Regla | Qué valida |
|---|---|
| R1 | El libro debe tener ejemplares disponibles |
| R2 | Máximo 3 préstamos activos por socio |
| R3 | No se presta a quien tenga préstamos vencidos |
| R4 | El plazo es de 15 días (lo pone la API) |
| R5 | Un préstamo no se devuelve dos veces |
| R6 | El socio debe estar activo |

## Azure

1. Crear la base en Azure SQL y abrir el **firewall** para tu IP (portal → el servidor SQL → Redes).
2. Conectarse a **esa base** en SSMS y ejecutar `database/BiblioTech.Azure.sql`
   (no usa `USE master` ni `CREATE DATABASE`, que Azure no permite).
3. La cadena de conexión de Azure va en **Secretos de usuario** (local) o en
   **App Service → Configuración → Cadenas de conexión** (publicado). **Nunca en appsettings.json.**
4. En Azure, poner también la variable `Jwt__Clave` con una clave propia de 32+ caracteres
   (la de `appsettings.Development.json` es solo para desarrollo).

## Puertos (si cambian, hay que actualizarlos)

| Archivo | Lleva el puerto de |
|---|---|
| `Api/Properties/launchSettings.json` | la API (7180) |
| `Web/Properties/launchSettings.json` | la Web (7093 / 5113) |
| `Web.Client/wwwroot/appsettings.json` → `ApiUrl` | la API |
| `Api/appsettings.json` → `Cors` | la Web |
| `Api/BiblioTech2.Api.http` → `@host` | la API |



## Etiquetas (puntos de avance)

| Tag | Qué contiene |
|---|---|
| `esqueleto` | Solución creada, paquetes instalados, carpetas vacías |
| `back-completo` | API terminada: Autores, Libros, Socios, Préstamos, Dashboard y Auth JWT |

