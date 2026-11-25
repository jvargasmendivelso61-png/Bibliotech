namespace BiblioTech.Api.DTOs.Auth;

/// <summary>Datos públicos de un usuario. La contraseña (ni su hash) NUNCA sale de la API.</summary>
public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
