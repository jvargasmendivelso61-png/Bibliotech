using BiblioTech.Api.DTOs.Autores;
using BiblioTech2.Api.Respuestas;

namespace BiblioTech.Api.Services.Interfaces;

public interface IAutorService
{
    Task<Resultado<List<AutorDto>>> ListarAsync();
    Task<Resultado<AutorDto>> ObtenerAsync(int id);
    Task<Resultado<AutorDto>> CrearAsync(AutorGuardarDto dto);
    Task<Resultado> ActualizarAsync(int id, AutorGuardarDto dto);
    Task<Resultado> EliminarAsync(int id);
}