using BiblioTech.Api.DTOs.Auth;
using BiblioTech2.Api.Models;

namespace BiblioTech.Api.Services.Interfaces;

public interface ITokenService
{
    /// <summary>Crea el JWT firmado para un usuario que ya inició sesión.</summary>
    TokenDto CrearToken(Usuario usuario);
}
