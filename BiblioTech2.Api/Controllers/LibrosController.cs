using BiblioTech.Api.DTOs.Libros;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech2.Api.Controllers;

[Route("api/[controller]")]
public class LibrosController(ILibroService servicio) : ApiControllerBase
{
    /// <summary>GET api/libros?buscar=clean</summary>
    [HttpGet]
    public async Task<ActionResult<RespuestaApi<List<LibroDto>>>> Listar([FromQuery] string? buscar)
        => Responder(await servicio.ListarAsync(buscar));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RespuestaApi<LibroDto>>> Obtener(int id)
        => Responder(await servicio.ObtenerAsync(id));

    [Authorize]   // 🔒 escribir requiere token
    [HttpPost]
    public async Task<ActionResult<RespuestaApi<LibroDto>>> Crear(LibroGuardarDto dto)
    {
        var resultado = await servicio.CrearAsync(dto);
        if (!resultado.Exito)
            return Responder(resultado);

        return CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.Id },
            RespuestaApi<LibroDto>.Desde(resultado));
    }

    [Authorize]   // 🔒 escribir requiere token
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RespuestaApi<object>>> Actualizar(int id, LibroGuardarDto dto)
        => Responder(await servicio.ActualizarAsync(id, dto));

    [Authorize]   // 🔒 escribir requiere token
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RespuestaApi<object>>> Eliminar(int id)
        => Responder(await servicio.EliminarAsync(id));
}
