using BiblioTech.Api.DTOs.Libros;
using BiblioTech2.Api.Respuestas;

namespace BiblioTech.Api.Services.Interfaces;

public interface ILibroService
{
    Task<Resultado<List<LibroDto>>> ListarAsync(string? buscar);
    Task<Resultado<LibroDto>> ObtenerAsync(int id);
    Task<Resultado<LibroDto>> CrearAsync(LibroGuardarDto dto);
    Task<Resultado> ActualizarAsync(int id, LibroGuardarDto dto);
    Task<Resultado> EliminarAsync(int id);
}
