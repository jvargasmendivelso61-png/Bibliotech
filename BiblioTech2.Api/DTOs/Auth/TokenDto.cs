namespace BiblioTech.Api.DTOs.Auth;

/// <summary>Lo que la API DEVUELVE al iniciar sesión: el token y para quién es.</summary>
public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
