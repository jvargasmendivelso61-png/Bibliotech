using BiblioTech.Api.DTOs.Dashboard;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Data;
using BiblioTech2.Api.Respuestas;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Api.Services;

public class DashboardService(BibliotecaContext db) : IDashboardService
{
    public async Task<Resultado<ResumenDashboardDto>> ObtenerResumenAsync()
    {
        // SqlQuery ejecuta SQL propio y llena el DTO por NOMBRE de columna.
        // Un EXEC no se puede combinar con más LINQ, por eso primero ToListAsync() y luego First().
        var filas = await db.Database
            .SqlQuery<ResumenDashboardDto>($"EXEC dbo.sp_ResumenDashboard")
            .ToListAsync();

        return Resultado<ResumenDashboardDto>.ok(filas.First(), MensajesDashboard.Resumen);
    }
}
