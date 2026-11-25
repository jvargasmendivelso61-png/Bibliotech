using System.ComponentModel.DataAnnotations;

namespace BiblioTech.Api.DTOs.Socios;

/// <summary>Lo que la API RECIBE para crear o editar un socio (entrada).</summary>
public class SocioGuardarDto
{
    [Required(ErrorMessage = "El documento es obligatorio.")]
    [RegularExpression(@"^[0-9]{5,20}$", ErrorMessage = "El documento debe tener solo números (entre 5 y 20).")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre admite máximo 120 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    // Misma idea que el CHECK de la base (CK_Socios_Email): algo@algo.algo
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(150, ErrorMessage = "El correo admite máximo 150 caracteres.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El correo no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;
}
