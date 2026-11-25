using System.Text;
using BiblioTech.Api.DTOs.Auth;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Configuracion;
using BiblioTech2.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BiblioTech.Api.Services;

/// <summary>
/// Fabrica el JWT. Un JWT tiene 3 partes separadas por puntos:
///   encabezado . datos (claims) . firma
/// Los datos se pueden LEER (no están cifrados), pero no se pueden CAMBIAR
/// sin romper la firma, porque la firma se hace con la Clave secreta de la API.
/// </summary>
public class TokenService(IOptions<JwtOpciones> opciones) : ITokenService
{
    private readonly JwtOpciones jwt = opciones.Value;

    public TokenDto CrearToken(Usuario usuario)
    {
        var expira = DateTime.UtcNow.AddMinutes(jwt.MinutosExpiracion);
        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave));

        var descriptor = new SecurityTokenDescriptor
        {
            // Claims: los datos que viajan dentro del token (nombres cortos estándar).
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),   // "sub"  = id del usuario
                [JwtRegisteredClaimNames.Email] = usuario.Email,         // "email"
                ["role"] = usuario.Rol                                    // "role" = Admin | Bibliotecario
            },
            Issuer = jwt.Emisor,
            Audience = jwt.Audiencia,
            Expires = expira,
            SigningCredentials = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256)
        };

        return new TokenDto
        {
            Token = new JsonWebTokenHandler().CreateToken(descriptor),
            Expira = expira,
            Email = usuario.Email,
            Rol = usuario.Rol
        };
    }
}
