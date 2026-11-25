using System.ComponentModel.DataAnnotations;

namespace BiblioTech.Api.DTOs.Socios;

/// <summary>Cuerpo de PATCH api/socios/{id}/estado → { "activo": false }</summary>
public class SocioEstadoDto
{
    [Required(ErrorMessage = "Debes indicar si el socio queda activo (true) o inactivo (false).")]
    public bool? Activo { get; set; }
}
