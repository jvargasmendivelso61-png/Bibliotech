using BiblioTech.Api.DTOs.Dashboard;
using BiblioTech2.Api.Respuestas;

namespace BiblioTech.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<Resultado<ResumenDashboardDto>> ObtenerResumenAsync();
}
