namespace BiblioTech.Api.DTOs.Prestamos;

/// <summary>Lo que la API DEVUELVE de un préstamo (salida).</summary>
public class PrestamoDto
{
    public int Id { get; set; }
    public int LibroId { get; set; }
    public string LibroTitulo { get; set; } = string.Empty;
    public int SocioId { get; set; }
    public string SocioNombre { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaDevolucionEsperada { get; set; }
    public DateTime? FechaDevolucionReal { get; set; }
    public string Estado { get; set; } = string.Empty;

    /// <summary>Calculado: activo y con la fecha esperada ya pasada.</summary>
    public bool Vencido { get; set; }
    public int DiasRetraso { get; set; }
}
