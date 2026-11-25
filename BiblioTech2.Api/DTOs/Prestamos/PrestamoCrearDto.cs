using System.ComponentModel.DataAnnotations;

namespace BiblioTech.Api.DTOs.Prestamos;

/// <summary>
/// Lo que la API RECIBE para prestar un libro. Las fechas NO se reciben:
/// las pone la API (hoy y hoy + 15 días, regla R4).
/// </summary>
public class PrestamoCrearDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debes indicar el libro.")]
    public int LibroId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debes indicar el socio.")]
    public int SocioId { get; set; }
}
