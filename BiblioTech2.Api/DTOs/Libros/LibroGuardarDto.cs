using System.ComponentModel.DataAnnotations;

namespace BiblioTech.Api.DTOs.Libros;

/// <summary>Lo que la API RECIBE para crear o editar un libro (entrada).</summary>
public class LibroGuardarDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, ErrorMessage = "El título admite máximo 200 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es obligatorio.")]
    [StringLength(20, ErrorMessage = "El ISBN admite máximo 20 caracteres.")]
    public string Isbn { get; set; } = string.Empty;

    [Range(1450, 2100, ErrorMessage = "El año debe estar entre 1450 y 2100.")]
    public int? AnioPublicacion { get; set; }

    [Range(1, 1000, ErrorMessage = "Los ejemplares deben estar entre 1 y 1000.")]
    public int EjemplaresTotales { get; set; }

    /// <summary>Ids de los autores del libro (relación N:N con la tabla LibroAutor).</summary>
    [MinLength(1, ErrorMessage = "El libro debe tener al menos un autor.")]
    public List<int> AutorIds { get; set; } = [];
}
