using BiblioTech.Api.DTOs.Socios;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech2.Api.Controllers;

[Route("api/[controller]")]
public class SociosController(ISocioService servicio) : ApiControllerBase
{
    /// <summary>GET api/socios?activos=true</summary>
    [HttpGet]
    public async Task<ActionResult<RespuestaApi<List<SocioDto>>>> Listar([FromQuery] bool? activos)
        => Responder(await servicio.ListarAsync(activos));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RespuestaApi<SocioDto>>> Obtener(int id)
        => Responder(await servicio.ObtenerAsync(id));

    [Authorize]   // 🔒 escribir requiere token
    [HttpPost]
    public async Task<ActionResult<RespuestaApi<SocioDto>>> Crear(SocioGuardarDto dto)
    {
        var resultado = await servicio.CrearAsync(dto);
        if (!resultado.Exito)
            return Responder(resultado);

        return CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.Id },
            RespuestaApi<SocioDto>.Desde(resultado));
    }

    [Authorize]   // 🔒 escribir requiere token
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RespuestaApi<object>>> Actualizar(int id, SocioGuardarDto dto)
        => Responder(await servicio.ActualizarAsync(id, dto));

    /// <summary>PATCH api/socios/5/estado  { "activo": false }  → activar o desactivar (no se eliminan).</summary>
    [Authorize]   // 🔒 escribir requiere token
    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult<RespuestaApi<object>>> CambiarEstado(int id, SocioEstadoDto dto)
        => Responder(await servicio.CambiarEstadoAsync(id, dto.Activo!.Value));
}
