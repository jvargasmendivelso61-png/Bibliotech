using BiblioTech2.Api.Respuestas;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech2.Api.Middleware;

/// <summary>Red de seguridad: SOLO atrapa errores inesperados (BD caída, bug, restricción violada).</summary>
public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private static readonly Dictionary<string, string> MensajeRestricciones = new()
    {
        ["FK_LibroAutor_Autores"] = "El autor tiene libros asociados.",
        ["UQ_Socios_Documento"] = "Ya existe un socio con este documento.",
        ["UQ_Socios_Email"] = "Ya existe un socio con este correo.",
        ["FK_Prestamos_Libros"] = "El libro tiene préstamos asociados.",
        ["FK_Prestamos_Socios"] = "El socio tiene préstamos asociados.",
        ["CK_Libros_Disponibles"] = "Los ejemplares no pueden ser negativos ni mayores que los totales.",
        ["UQ_Libros_ISBN"] = "Ya existe un libro con ese ISBN.",
        ["CK_Socios_Email"] = "El correo no tiene un formato válido."
    };

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await next(contexto);
        }
        catch (Exception ex) when (!contexto.Response.HasStarted)
        {
            await ResponderAsync(contexto, ex);
        }
    }

    private async Task ResponderAsync(HttpContext contexto, Exception ex)
    {
        var (codigo, mensaje) = ex switch
        {
            DbUpdateException e => (StatusCodes.Status409Conflict, TraducirErrorBaseDatos(e)),
            _ => (StatusCodes.Status500InternalServerError,
                  "Ocurrió un error inesperado. Intenta de nuevo más tarde.")
        };

        if (codigo == StatusCodes.Status500InternalServerError)
            logger.LogError(ex, "Error no controlado en {Ruta}", contexto.Request.Path);
        else
            logger.LogWarning("{Codigo} en {Ruta}: {Mensaje}", codigo, contexto.Request.Path, mensaje);

        contexto.Response.StatusCode = codigo;
        await contexto.Response.WriteAsJsonAsync(new RespuestaApi<object>
        {
            Exito = false,
            Mensaje = mensaje
        });
    }

    private static string TraducirErrorBaseDatos(DbUpdateException ex)
    {
        var mensajeSql = ex.InnerException?.Message ?? ex.Message;
        foreach (var (restriccion, mensaje) in MensajeRestricciones)
        {
            if (mensajeSql.Contains(restriccion, StringComparison.OrdinalIgnoreCase))
                return mensaje;
        }
        return "La operación viola una restricción de la base de datos.";
    }
}
