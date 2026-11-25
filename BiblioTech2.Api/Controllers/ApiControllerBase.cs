using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech2.Api.Controllers
{
    /// <summary>
    /// Base de TODOS los controladores: convierte un Resultado del servicio
    /// en código HTTP + sobre RespuestaApi. Así ningún controlador repite esa lógica.
    /// </summary>
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult Responder<T>(Resultado<T> resultado)
            => StatusCode(CodigoHttp(resultado), RespuestaApi<T>.Desde(resultado));

        protected ActionResult Responder(Resultado resultado)
            => StatusCode(CodigoHttp(resultado), RespuestaApi<object>.Desde(resultado));

        private static int CodigoHttp(Resultado resultado) => resultado.Tipo switch
        {
            TipoResultado.Exito => StatusCodes.Status200OK,
            TipoResultado.NoEncontrado => StatusCodes.Status404NotFound,
            TipoResultado.ReglaNegocio => StatusCodes.Status409Conflict,
            TipoResultado.Validacion => StatusCodes.Status400BadRequest,
            TipoResultado.NoAutorizado => StatusCodes.Status401Unauthorized,
            TipoResultado.Prohibido => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
