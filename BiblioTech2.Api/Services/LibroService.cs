using BiblioTech.Api.DTOs.Libros;
using BiblioTech.Api.Services.Interfaces;
using BiblioTech2.Api.Constants;
using BiblioTech2.Api.Data;
using BiblioTech2.Api.Models;
using BiblioTech2.Api.Respuestas;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Api.Services;

public class LibroService(BibliotecaContext db) : ILibroService
{
    public async Task<Resultado<List<LibroDto>>> ListarAsync(string? buscar)
    {
        var consulta = db.Libros.AsQueryable();

        // Búsqueda opcional: por título, ISBN o nombre de autor.
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            consulta = consulta.Where(l =>
                l.Titulo.Contains(texto) ||
                l.Isbn.Contains(texto) ||
                l.Autores.Any(a => a.Nombre.Contains(texto)));
        }

        var libros = await consulta
            .OrderBy(l => l.Titulo)
            .Select(ADto)
            .ToListAsync();

        return Resultado<List<LibroDto>>.ok(libros, MensajesLibro.Listado);
    }

    public async Task<Resultado<LibroDto>> ObtenerAsync(int id)
    {
        var libro = await BuscarDtoAsync(id);
        if (libro is null)
            return Resultado<LibroDto>.NoEncontrado(MensajesLibro.NoExiste(id));

        return Resultado<LibroDto>.ok(libro, MensajesLibro.Encontrado);
    }

    public async Task<Resultado<LibroDto>> CrearAsync(LibroGuardarDto dto)
    {
        var isbn = dto.Isbn.Trim();
        if (await db.Libros.AnyAsync(l => l.Isbn == isbn))
            return Resultado<LibroDto>.ReglaNegocio(MensajesLibro.IsbnDuplicado(isbn));

        var (autores, faltantes) = await CargarAutoresAsync(dto.AutorIds);
        if (faltantes.Count > 0)
            return Resultado<LibroDto>.Validacion(MensajesLibro.AutoresNoExisten(faltantes));

        var libro = new Libro
        {
            Titulo = dto.Titulo.Trim(),
            Isbn = isbn,
            AnioPublicacion = dto.AnioPublicacion,
            EjemplaresTotales = dto.EjemplaresTotales,
            EjemplaresDisponibles = dto.EjemplaresTotales,   // libro nuevo: todos disponibles
            Autores = autores
        };

        db.Libros.Add(libro);
        await db.SaveChangesAsync();

        return Resultado<LibroDto>.ok((await BuscarDtoAsync(libro.Id))!, MensajesLibro.Creado);
    }

    public async Task<Resultado> ActualizarAsync(int id, LibroGuardarDto dto)
    {
        var libro = await db.Libros.Include(l => l.Autores).FirstOrDefaultAsync(l => l.Id == id);
        if (libro is null)
            return Resultado.NoEncontrado(MensajesLibro.NoExiste(id));

        var isbn = dto.Isbn.Trim();
        if (await db.Libros.AnyAsync(l => l.Isbn == isbn && l.Id != id))
            return Resultado.ReglaNegocio(MensajesLibro.IsbnDuplicado(isbn));

        var (autores, faltantes) = await CargarAutoresAsync(dto.AutorIds);
        if (faltantes.Count > 0)
            return Resultado.Validacion(MensajesLibro.AutoresNoExisten(faltantes));

        // Los ejemplares prestados ahora no se pueden "desaparecer".
        var prestados = await db.Prestamos
            .CountAsync(p => p.LibroId == id && p.Estado == ReglasPrestamo.EstadoActivo);
        if (dto.EjemplaresTotales < prestados)
            return Resultado.ReglaNegocio(MensajesLibro.TotalesMenoresQuePrestados(prestados));

        libro.Titulo = dto.Titulo.Trim();
        libro.Isbn = isbn;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.EjemplaresTotales = dto.EjemplaresTotales;
        libro.EjemplaresDisponibles = dto.EjemplaresTotales - prestados;

        // Sincronizar autores: quitar los que ya no están y agregar los nuevos.
        foreach (var quitar in libro.Autores.Where(a => !autores.Contains(a)).ToList())
            libro.Autores.Remove(quitar);
        foreach (var agregar in autores.Where(a => !libro.Autores.Contains(a)))
            libro.Autores.Add(agregar);

        await db.SaveChangesAsync();

        return Resultado.ok(MensajesLibro.Actualizado);
    }

    public async Task<Resultado> EliminarAsync(int id)
    {
        var libro = await db.Libros.FindAsync(id);
        if (libro is null)
            return Resultado.NoEncontrado(MensajesLibro.NoExiste(id));

        // El historial de préstamos se conserva: un libro con préstamos no se borra.
        var prestamos = await db.Prestamos.CountAsync(p => p.LibroId == id);
        if (prestamos > 0)
            return Resultado.ReglaNegocio(MensajesLibro.TienePrestamos(prestamos));

        db.Libros.Remove(libro);   // LibroAutor se borra solo (ON DELETE CASCADE)
        await db.SaveChangesAsync();

        return Resultado.ok(MensajesLibro.Eliminado);
    }

    // ---------- Ayudantes privados ----------

    /// <summary>Proyección Libro → LibroDto (EF la traduce a un solo SELECT).</summary>
    private static readonly System.Linq.Expressions.Expression<Func<Libro, LibroDto>> ADto = l => new LibroDto
    {
        Id = l.Id,
        Titulo = l.Titulo,
        Isbn = l.Isbn,
        AnioPublicacion = l.AnioPublicacion,
        EjemplaresTotales = l.EjemplaresTotales,
        EjemplaresDisponibles = l.EjemplaresDisponibles,
        Autores = l.Autores
            .OrderBy(a => a.Nombre)
            .Select(a => new AutorResumenDto { Id = a.Id, Nombre = a.Nombre })
            .ToList()
    };

    private Task<LibroDto?> BuscarDtoAsync(int id)
        => db.Libros.Where(l => l.Id == id).Select(ADto).FirstOrDefaultAsync();

    /// <summary>Trae los autores pedidos y devuelve también los ids que no existen.</summary>
    private async Task<(List<Autor> Autores, List<int> Faltantes)> CargarAutoresAsync(List<int> ids)
    {
        var unicos = ids.Distinct().ToList();
        var autores = await db.Autores.Where(a => unicos.Contains(a.Id)).ToListAsync();
        var faltantes = unicos.Except(autores.Select(a => a.Id)).ToList();
        return (autores, faltantes);
    }
}
