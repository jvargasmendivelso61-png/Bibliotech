namespace BiblioTech.Api.DTOs.Libros;

/// <summary>Lo que la API DEVUELVE de un libro (salida).</summary>
public class LibroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int? AnioPublicacion { get; set; }
    public int EjemplaresTotales { get; set; }
    public int EjemplaresDisponibles { get; set; }
    public List<AutorResumenDto> Autores { get; set; } = [];
}

/// <summary>Solo lo necesario de cada autor para mostrarlo dentro del libro.</summary>
public class AutorResumenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
