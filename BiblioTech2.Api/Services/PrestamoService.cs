using BiblioTech.Api.DTOs.Prestamos;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Data;
using BiblioTech2.Api.Models;
using BiblioTech2.Api.Respuestas;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Api.Services;

/// <summary>
/// Préstamos: aquí viven las reglas de negocio de la biblioteca.
///   R1 el libro debe tener ejemplares disponibles
///   R2 máximo 3 préstamos activos por socio
///   R3 no se presta a quien tenga préstamos vencidos
///   R4 el plazo es de 15 días
///   R5 un préstamo no se devuelve dos veces
///   R6 el socio debe estar activo
/// </summary>
public class PrestamoService(BibliotecaContext db) : IPrestamoService
{
    private const string EstadoVencido = "Vencido";

    public async Task<Resultado<List<PrestamoDto>>> ListarAsync(string? estado, int? socioId)
    {
        var ahora = DateTime.Now;
        var consulta = db.Prestamos.AsQueryable();

        if (socioId.HasValue)
            consulta = consulta.Where(p => p.SocioId == socioId.Value);

        // Filtro opcional: Activo, Devuelto o Vencido (Vencido = activo con fecha pasada).
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (estado.Equals(ReglasPrestamo.EstadoActivo, StringComparison.OrdinalIgnoreCase))
                consulta = consulta.Where(p => p.Estado == ReglasPrestamo.EstadoActivo);
            else if (estado.Equals(ReglasPrestamo.EstadoDevuelto, StringComparison.OrdinalIgnoreCase))
                consulta = consulta.Where(p => p.Estado == ReglasPrestamo.EstadoDevuelto);
            else if (estado.Equals(EstadoVencido, StringComparison.OrdinalIgnoreCase))
                consulta = consulta.Where(p => p.Estado == ReglasPrestamo.EstadoActivo && p.FechaDevolucionEsperada < ahora);
            else
                return Resultado<List<PrestamoDto>>.Validacion(MensajesPrestamo.EstadoInvalido(estado));
        }

        var prestamos = await consulta
            .OrderByDescending(p => p.FechaPrestamo)
            .Select(ADto(ahora))
            .ToListAsync();

        return Resultado<List<PrestamoDto>>.ok(prestamos, MensajesPrestamo.Listado);
    }

    public async Task<Resultado<PrestamoDto>> ObtenerAsync(int id)
    {
        var prestamo = await BuscarDtoAsync(id);
        if (prestamo is null)
            return Resultado<PrestamoDto>.NoEncontrado(MensajesPrestamo.NoExiste(id));

        return Resultado<PrestamoDto>.ok(prestamo, MensajesPrestamo.Encontrado);
    }

    /// <summary>Lee la VISTA vw_PrestamosVencidos de la base de datos.</summary>
    public async Task<Resultado<List<PrestamoVencidoDto>>> ListarVencidosAsync()
    {
        var vencidos = await db.VwPrestamosVencidos
            .OrderByDescending(v => v.DiasRetraso)
            .Select(v => new PrestamoVencidoDto
            {
                PrestamoId = v.PrestamoId,
                Titulo = v.Titulo,
                Socio = v.Socio,
                Email = v.Email,
                FechaPrestamo = v.FechaPrestamo,
                FechaDevolucionEsperada = v.FechaDevolucionEsperada,
                DiasRetraso = v.DiasRetraso ?? 0
            })
            .ToListAsync();

        return Resultado<List<PrestamoVencidoDto>>.ok(vencidos, MensajesPrestamo.Vencidos);
    }

    public async Task<Resultado<PrestamoDto>> PrestarAsync(PrestamoCrearDto dto)
    {
        var ahora = DateTime.Now;

        var socio = await db.Socios.FindAsync(dto.SocioId);
        if (socio is null)
            return Resultado<PrestamoDto>.NoEncontrado(MensajesPrestamo.SocioNoExiste(dto.SocioId));

        var libro = await db.Libros.FindAsync(dto.LibroId);
        if (libro is null)
            return Resultado<PrestamoDto>.NoEncontrado(MensajesPrestamo.LibroNoExiste(dto.LibroId));

        // R6: socio activo
        if (!socio.Activo)
            return Resultado<PrestamoDto>.ReglaNegocio(MensajesPrestamo.SocioInactivo);

        var activosDelSocio = db.Prestamos
            .Where(p => p.SocioId == socio.Id && p.Estado == ReglasPrestamo.EstadoActivo);

        // R3: sin préstamos vencidos
        var vencidos = await activosDelSocio.CountAsync(p => p.FechaDevolucionEsperada < ahora);
        if (vencidos > 0)
            return Resultado<PrestamoDto>.ReglaNegocio(MensajesPrestamo.TieneVencidos(vencidos));

        // R2: máximo de préstamos activos
        if (await activosDelSocio.CountAsync() >= ReglasPrestamo.MaximoPrestamosActivos)
            return Resultado<PrestamoDto>.ReglaNegocio(MensajesPrestamo.MaximoPrestamos(ReglasPrestamo.MaximoPrestamosActivos));

        // R1: ejemplares disponibles
        if (libro.EjemplaresDisponibles <= 0)
            return Resultado<PrestamoDto>.ReglaNegocio(MensajesPrestamo.SinDisponibles(libro.Titulo));

        var prestamo = new Prestamo
        {
            LibroId = libro.Id,
            SocioId = socio.Id,
            FechaPrestamo = ahora,
            FechaDevolucionEsperada = ahora.AddDays(ReglasPrestamo.PlazoDias),   // R4
            Estado = ReglasPrestamo.EstadoActivo
        };

        db.Prestamos.Add(prestamo);
        libro.EjemplaresDisponibles--;

        // Un solo SaveChanges = una sola transacción: se guardan los dos cambios o ninguno.
        await db.SaveChangesAsync();

        return Resultado<PrestamoDto>.ok((await BuscarDtoAsync(prestamo.Id))!, MensajesPrestamo.Creado);
    }

    public async Task<Resultado> DevolverAsync(int id)
    {
        var prestamo = await db.Prestamos.Include(p => p.Libro).FirstOrDefaultAsync(p => p.Id == id);
        if (prestamo is null)
            return Resultado.NoEncontrado(MensajesPrestamo.NoExiste(id));

        // R5: no se devuelve dos veces
        if (prestamo.Estado == ReglasPrestamo.EstadoDevuelto)
            return Resultado.ReglaNegocio(MensajesPrestamo.YaDevuelto);

        var ahora = DateTime.Now;
        var diasRetraso = Math.Max(0, (ahora.Date - prestamo.FechaDevolucionEsperada.Date).Days);

        prestamo.Estado = ReglasPrestamo.EstadoDevuelto;
        prestamo.FechaDevolucionReal = ahora;
        prestamo.Libro.EjemplaresDisponibles++;

        await db.SaveChangesAsync();

        return Resultado.ok(MensajesPrestamo.Devuelto(diasRetraso));
    }

    // ---------- Ayudantes privados ----------

    private static System.Linq.Expressions.Expression<Func<Prestamo, PrestamoDto>> ADto(DateTime ahora) => p => new PrestamoDto
    {
        Id = p.Id,
        LibroId = p.LibroId,
        LibroTitulo = p.Libro.Titulo,
        SocioId = p.SocioId,
        SocioNombre = p.Socio.Nombre,
        FechaPrestamo = p.FechaPrestamo,
        FechaDevolucionEsperada = p.FechaDevolucionEsperada,
        FechaDevolucionReal = p.FechaDevolucionReal,
        Estado = p.Estado,
        Vencido = p.Estado == ReglasPrestamo.EstadoActivo && p.FechaDevolucionEsperada < ahora,
        DiasRetraso = p.Estado == ReglasPrestamo.EstadoActivo && p.FechaDevolucionEsperada < ahora
            ? EF.Functions.DateDiffDay(p.FechaDevolucionEsperada, ahora)
            : 0
    };

    private Task<PrestamoDto?> BuscarDtoAsync(int id)
        => db.Prestamos.Where(p => p.Id == id).Select(ADto(DateTime.Now)).FirstOrDefaultAsync();
}
