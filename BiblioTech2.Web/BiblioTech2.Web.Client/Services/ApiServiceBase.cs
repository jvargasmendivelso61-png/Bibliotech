using System.Net.Http.Json;
using System.Text.Json;
using BiblioTech2.Web.Client.Models;

namespace BiblioTech2.Web.Client.Services;

public abstract class ApiServiceBase(HttpClient http)
{
    protected HttpClient Http { get; } = http;

    protected async Task<RespuestaApi<T>> EnviarAsync<T>(Func<Task<HttpResponseMessage>> peticion)
    {
        try
        {
            using var respuesta = await peticion();

            // Aunque el código sea 400/401/404/409/500, nuestra API manda el sobre en el cuerpo.
            var cuerpo = await respuesta.Content.ReadFromJsonAsync<RespuestaApi<T>>();
            return cuerpo ?? RespuestaApi<T>.Fallo($"La API respondió {(int)respuesta.StatusCode} sin contenido.");
        }
        catch (HttpRequestException)
        {
            // API apagada, puerto equivocado o CORS bloqueado: el navegador solo dice "Failed to fetch".
            return RespuestaApi<T>.Fallo("No fue posible conectar con la API. ¿Está en ejecución?");
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            // JsonException: JSON mal formado. NotSupportedException: la respuesta no era JSON (ej. una página HTML de error).
            return RespuestaApi<T>.Fallo("La API devolvió una respuesta con un formato inesperado.");
        }
    }
}
