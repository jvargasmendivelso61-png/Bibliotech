namespace BiblioTech.Api.DTOs.Dashboard;

/// <summary>
/// Resultado del procedimiento dbo.sp_ResumenDashboard.
/// Los nombres de las propiedades deben ser IGUALES a las columnas que devuelve el SELECT.
/// </summary>
public class ResumenDashboardDto
{
    public int TotalLibros { get; set; }
    public int EjemplaresDisponibles { get; set; }
    public int PrestamosActivos { get; set; }
    public int PrestamosVencidos { get; set; }
    public int SociosActivos { get; set; }
}
