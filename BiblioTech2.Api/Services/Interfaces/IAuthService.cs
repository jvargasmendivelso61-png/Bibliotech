using BiblioTech.Api.DTOs.Auth;
using BiblioTech2.Api.Respuestas;

namespace BiblioTech.Api.Services.Interfaces;

public interface IAuthService
{
    Task<Resultado<TokenDto>> LoginAsync(LoginDto dto);

    /// <param name="autenticado">¿Quien llama envió un token válido?</param>
    /// <param name="esAdmin">¿Quien llama tiene el rol Admin?</param>
    Task<Resultado<UsuarioDto>> RegistrarAsync(RegistroDto dto, bool autenticado, bool esAdmin);

    Task<Resultado<UsuarioDto>> ObtenerPerfilAsync(int id);
}
