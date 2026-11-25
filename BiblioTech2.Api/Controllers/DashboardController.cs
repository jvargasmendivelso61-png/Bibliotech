using BiblioTech.Api.DTOs.Dashboard;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Mvc;

namespace BiblioTech2.Api.Controllers;

[Route("api/[controller]")]
public class DashboardController(IDashboardService servicio) : ApiControllerBase
{
    /// <summary>GET api/dashboard → ejecuta el procedimiento sp_ResumenDashboard</summary>
    [HttpGet]
    public async Task<ActionResult<RespuestaApi<ResumenDashboardDto>>> Resumen()
        => Responder(await servicio.ObtenerResumenAsync());
}
