using BiblioTech.Api.DTOs.Socios;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Data;
using BiblioTech2.Api.Models;
using BiblioTech2.Api.Respuestas;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Api.Services;

/// <summary>
/// Los socios NO se eliminan (tienen historial de préstamos): se activan o desactivan.
/// </summary>
public class SocioService(BibliotecaContext db) : ISocioService
{
    public async Task<Resultado<List<SocioDto>>> ListarAsync(bool? activos)
    {
        var consulta = db.Socios.AsQueryable();
        if (activos.HasValue)
            consulta = consulta.Where(s => s.Activo == activos.Value);

        var socios = await consulta
            .OrderBy(s => s.Nombre)
            .Select(ADto)
            .ToListAsync();

        return Resultado<List<SocioDto>>.ok(socios, MensajesSocio.Listado);
    }

    public async Task<Resultado<SocioDto>> ObtenerAsync(int id)
    {
        var socio = await BuscarDtoAsync(id);
        if (socio is null)
            return Resultado<SocioDto>.NoEncontrado(MensajesSocio.NoExiste(id));

        return Resultado<SocioDto>.ok(socio, MensajesSocio.Encontrado);
    }

    public async Task<Resultado<SocioDto>> CrearAsync(SocioGuardarDto dto)
    {
        var documento = dto.Documento.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await db.Socios.AnyAsync(s => s.Documento == documento))
            return Resultado<SocioDto>.ReglaNegocio(MensajesSocio.DocumentoDuplicado(documento));
        if (await db.Socios.AnyAsync(s => s.Email == email))
            return Resultado<SocioDto>.ReglaNegocio(MensajesSocio.EmailDuplicado(email));

        var socio = new Socio
        {
            Documento = documento,
            Nombre = dto.Nombre.Trim(),
            Email = email,
            FechaRegistro = DateTime.Now,
            Activo = true
        };

        db.Socios.Add(socio);
        await db.SaveChangesAsync();

        return Resultado<SocioDto>.ok((await BuscarDtoAsync(socio.Id))!, MensajesSocio.Creado);
    }

    public async Task<Resultado> ActualizarAsync(int id, SocioGuardarDto dto)
    {
        var socio = await db.Socios.FindAsync(id);
        if (socio is null)
            return Resultado.NoEncontrado(MensajesSocio.NoExiste(id));

        var documento = dto.Documento.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await db.Socios.AnyAsync(s => s.Documento == documento && s.Id != id))
            return Resultado.ReglaNegocio(MensajesSocio.DocumentoDuplicado(documento));
        if (await db.Socios.AnyAsync(s => s.Email == email && s.Id != id))
            return Resultado.ReglaNegocio(MensajesSocio.EmailDuplicado(email));

        socio.Documento = documento;
        socio.Nombre = dto.Nombre.Trim();
        socio.Email = email;

        await db.SaveChangesAsync();

        return Resultado.ok(MensajesSocio.Actualizado);
    }

    public async Task<Resultado> CambiarEstadoAsync(int id, bool activo)
    {
        var socio = await db.Socios.FindAsync(id);
        if (socio is null)
            return Resultado.NoEncontrado(MensajesSocio.NoExiste(id));

        if (socio.Activo == activo)
            return Resultado.ReglaNegocio(MensajesSocio.YaEstaba(activo));

        // No se desactiva a alguien que todavía tiene libros en la casa.
        if (!activo)
        {
            var pendientes = await db.Prestamos
                .CountAsync(p => p.SocioId == id && p.Estado == ReglasPrestamo.EstadoActivo);
            if (pendientes > 0)
                return Resultado.ReglaNegocio(MensajesSocio.TienePrestamosActivos(pendientes));
        }

        socio.Activo = activo;
        await db.SaveChangesAsync();

        return Resultado.ok(activo ? MensajesSocio.Activado : MensajesSocio.Desactivado);
    }

    // ---------- Ayudantes privados ----------

    private static readonly System.Linq.Expressions.Expression<Func<Socio, SocioDto>> ADto = s => new SocioDto
    {
        Id = s.Id,
        Documento = s.Documento,
        Nombre = s.Nombre,
        Email = s.Email,
        FechaRegistro = s.FechaRegistro,
        Activo = s.Activo,
        PrestamosActivos = s.Prestamos.Count(p => p.Estado == ReglasPrestamo.EstadoActivo)
    };

    private Task<SocioDto?> BuscarDtoAsync(int id)
        => db.Socios.Where(s => s.Id == id).Select(ADto).FirstOrDefaultAsync();
}
