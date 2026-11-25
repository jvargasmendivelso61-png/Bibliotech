using BiblioTech.Api.DTOs.Prestamos;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech2.Api.Controllers;

[Route("api/[controller]")]
public class PrestamosController(IPrestamoService servicio) : ApiControllerBase
{
    /// <summary>GET api/prestamos?estado=Activo|Devuelto|Vencido&amp;socioId=1</summary>
    [HttpGet]
    public async Task<ActionResult<RespuestaApi<List<PrestamoDto>>>> Listar([FromQuery] string? estado, [FromQuery] int? socioId)
        => Responder(await servicio.ListarAsync(estado, socioId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RespuestaApi<PrestamoDto>>> Obtener(int id)
        => Responder(await servicio.ObtenerAsync(id));

    /// <summary>GET api/prestamos/vencidos → lee la vista vw_PrestamosVencidos</summary>
    [HttpGet("vencidos")]
    public async Task<ActionResult<RespuestaApi<List<PrestamoVencidoDto>>>> Vencidos()
        => Responder(await servicio.ListarVencidosAsync());

    /// <summary>POST api/prestamos  { "libroId": 2, "socioId": 4 }  → prestar</summary>
    [Authorize]   // 🔒 escribir requiere token
    [HttpPost]
    public async Task<ActionResult<RespuestaApi<PrestamoDto>>> Prestar(PrestamoCrearDto dto)
    {
        var resultado = await servicio.PrestarAsync(dto);
        if (!resultado.Exito)
            return Responder(resultado);

        return CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.Id },
            RespuestaApi<PrestamoDto>.Desde(resultado));
    }

    /// <summary>PUT api/prestamos/5/devolver → registrar la devolución</summary>
    [Authorize]   // 🔒 escribir requiere token
    [HttpPut("{id:int}/devolver")]
    public async Task<ActionResult<RespuestaApi<object>>> Devolver(int id)
        => Responder(await servicio.DevolverAsync(id));
}
