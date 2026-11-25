using System.ComponentModel.DataAnnotations;

namespace BiblioTech.Api.DTOs.Auth;

/// <summary>Lo que la API RECIBE para registrar un usuario (bibliotecario o admin).</summary>
public class RegistroDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(150, ErrorMessage = "El correo admite máximo 150 caracteres.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El correo no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Opcional. Si no se envía, queda "Bibliotecario". El primer usuario siempre queda "Admin".</summary>
    [RegularExpression("^(Admin|Bibliotecario)$", ErrorMessage = "El rol debe ser Admin o Bibliotecario.")]
    public string? Rol { get; set; }
}
