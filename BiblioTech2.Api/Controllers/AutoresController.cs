using BiblioTech.Api.DTOs.Autores;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech2.Api.Controllers;

[Route("api/[controller]")]
public class AutoresController(IAutorService servicio) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RespuestaApi<List<AutorDto>>>> Listar()
        => Responder(await servicio.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RespuestaApi<AutorDto>>> Obtener(int id)
        => Responder(await servicio.ObtenerAsync(id));

    [Authorize]   // 🔒 escribir requiere token
    [HttpPost]
    public async Task<ActionResult<RespuestaApi<AutorDto>>> Crear(AutorGuardarDto dto)
    {
        var resultado = await servicio.CrearAsync(dto);
        if (!resultado.Exito)
            return Responder(resultado);

        return CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.Id },
            RespuestaApi<AutorDto>.Desde(resultado));
    }

    [Authorize]   // 🔒 escribir requiere token
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RespuestaApi<object>>> Actualizar(int id, AutorGuardarDto dto)
        => Responder(await servicio.ActualizarAsync(id, dto));

    [Authorize]   // 🔒 escribir requiere token
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RespuestaApi<object>>> Eliminar(int id)
        => Responder(await servicio.EliminarAsync(id));
}
