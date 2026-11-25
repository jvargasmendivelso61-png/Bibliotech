using System.Text;
using BiblioTech.Api.Services;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace BiblioTech2.Api.Configuracion;

/// <summary>
/// Métodos de extensión para que Program.cs quede corto y legible:
/// cada bloque de configuración vive aquí con su nombre.
/// </summary>
public static class ServiciosExtensions
{
    public const string PoliticaCors = "PermitirWeb";

    /// <summary>Registra un servicio por módulo (interfaz → implementación).</summary>
    public static IServiceCollection AddServiciosBiblioteca(this IServiceCollection services)
    {
        services.AddScoped<IAutorService, AutorService>();
        services.AddScoped<ILibroService, LibroService>();
        services.AddScoped<ISocioService, SocioService>();
        services.AddScoped<IPrestamoService, PrestamoService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }

    /// <summary>
    /// CORS: el navegador solo deja que la Web (otro puerto = otro "origen") llame a la API
    /// si la API lo autoriza. Los orígenes permitidos están en appsettings.json → "Cors".
    /// </summary>
    public static IServiceCollection AddCorsWeb(this IServiceCollection services, IConfiguration configuracion)
    {
        var origenes = configuracion.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];

        services.AddCors(opciones =>
            opciones.AddPolicy(PoliticaCors, politica => politica
                .WithOrigins(origenes)
                .AllowAnyHeader()
                .AllowAnyMethod()));

        return services;
    }

    /// <summary>
    /// Autenticación con JWT: la API revisa en cada petición el encabezado
    /// "Authorization: Bearer {token}" y valida firma, emisor, audiencia y vencimiento.
    /// </summary>
    public static IServiceCollection AddAutenticacionJwt(this IServiceCollection services, IConfiguration configuracion)
    {
        var seccion = configuracion.GetSection(JwtOpciones.Seccion);
        services.Configure<JwtOpciones>(seccion);

        var jwt = seccion.Get<JwtOpciones>() ?? new JwtOpciones();
        if (jwt.Clave.Length < 32)
            throw new InvalidOperationException(
                "Falta 'Jwt:Clave' (mínimo 32 caracteres). En desarrollo va en appsettings.Development.json; en Azure, en la variable Jwt__Clave.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opciones =>
            {
                opciones.MapInboundClaims = false;   // dejar los claims con su nombre corto: sub, email, role
                opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Emisor,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audiencia,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
                    NameClaimType = "email",
                    RoleClaimType = "role",                    // así funciona User.IsInRole("Admin")
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                // 401 y 403 también responden con el sobre RespuestaApi (no con una respuesta vacía).
                opciones.Events = new JwtBearerEvents
                {
                    OnChallenge = async contexto =>
                    {
                        contexto.HandleResponse();
                        contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await contexto.Response.WriteAsJsonAsync(new RespuestaApi<object>
                        {
                            Exito = false,
                            Mensaje = MensajesAuth.NoAutenticado
                        });
                    },
                    OnForbidden = async contexto =>
                    {
                        contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await contexto.Response.WriteAsJsonAsync(new RespuestaApi<object>
                        {
                            Exito = false,
                            Mensaje = MensajesAuth.SinPermiso
                        });
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }

    /// <summary>OpenAPI + esquema "Bearer": así Scalar muestra dónde pegar el token.</summary>
    public static IServiceCollection AddOpenApiConJwt(this IServiceCollection services)
    {
        services.AddOpenApi(opciones => opciones.AddDocumentTransformer((documento, _, _) =>
        {
            documento.Components ??= new OpenApiComponents();
            documento.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            documento.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Pega aquí el token que devuelve POST /api/auth/login"
            };
            documento.Security ??= [];
            documento.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", documento)] = []
            });
            return Task.CompletedTask;
        }));

        return services;
    }
}
