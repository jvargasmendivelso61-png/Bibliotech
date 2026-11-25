using BiblioTech.Api.DTOs.Auth;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BiblioTech2.Api.Controllers;

[Route("api/[controller]")]
public class AuthController(IAuthService servicio) : ApiControllerBase
{
    /// <summary>POST api/auth/login  { "email": "...", "password": "..." }  → devuelve el token</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RespuestaApi<TokenDto>>> Login(LoginDto dto)
        => Responder(await servicio.LoginAsync(dto));

    /// <summary>
    /// POST api/auth/registro. Sin token solo funciona para el PRIMER usuario (queda Admin);
    /// después hace falta el token de un Admin. Por eso es [AllowAnonymous] y el servicio decide.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("registro")]
    public async Task<ActionResult<RespuestaApi<UsuarioDto>>> Registrar(RegistroDto dto)
        => Responder(await servicio.RegistrarAsync(
            dto,
            autenticado: User.Identity?.IsAuthenticated ?? false,
            esAdmin: User.IsInRole(Roles.Admin)));

    /// <summary>GET api/auth/yo → ¿quién soy? (lee el id que viene dentro del token)</summary>
    [Authorize]
    [HttpGet("yo")]
    public async Task<ActionResult<RespuestaApi<UsuarioDto>>> Yo()
    {
        var id = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        return Responder(await servicio.ObtenerPerfilAsync(id));
    }
}
