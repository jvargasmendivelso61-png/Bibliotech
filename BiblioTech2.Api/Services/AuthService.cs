using BiblioTech.Api.DTOs.Auth;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Data;
using BiblioTech2.Api.Models;
using BiblioTech2.Api.Respuestas;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Api.Services;

/// <summary>
/// Login y registro. La contraseña se guarda con BCrypt (hash + sal):
/// ni nosotros podemos ver la contraseña original.
/// </summary>
public class AuthService(BibliotecaContext db, ITokenService tokens) : IAuthService
{
    public async Task<Resultado<TokenDto>> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        // BCrypt.Verify vuelve a calcular el hash con la sal guardada y lo compara.
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return Resultado<TokenDto>.NoAutorizado(MensajesAuth.CredencialesInvalidas);

        return Resultado<TokenDto>.ok(tokens.CrearToken(usuario), MensajesAuth.LoginExitoso);
    }

    public async Task<Resultado<UsuarioDto>> RegistrarAsync(RegistroDto dto, bool autenticado, bool esAdmin)
    {
        // Regla: el PRIMER usuario se registra sin token y queda Admin.
        // Desde el segundo, solo un Admin con token puede registrar.
        var esPrimero = !await db.Usuarios.AnyAsync();
        if (!esPrimero)
        {
            if (!autenticado)
                return Resultado<UsuarioDto>.NoAutorizado(MensajesAuth.NoAutenticado);
            if (!esAdmin)
                return Resultado<UsuarioDto>.Prohibido(MensajesAuth.SoloAdmin);
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.Email == email))
            return Resultado<UsuarioDto>.ReglaNegocio(MensajesAuth.EmailDuplicado(email));

        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = esPrimero ? Roles.Admin : (dto.Rol ?? Roles.Bibliotecario)
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        return Resultado<UsuarioDto>.ok(ADto(usuario), esPrimero ? MensajesAuth.PrimerAdmin : MensajesAuth.Registrado);
    }

    public async Task<Resultado<UsuarioDto>> ObtenerPerfilAsync(int id)
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null)
            return Resultado<UsuarioDto>.NoEncontrado(MensajesAuth.NoExiste(id));

        return Resultado<UsuarioDto>.ok(ADto(usuario), MensajesAuth.Perfil);
    }

    private static UsuarioDto ADto(Usuario u) => new() { Id = u.Id, Email = u.Email, Rol = u.Rol };
}
