using BiblioTech.Api.DTOs.Prestamos;
using BiblioTech2.Api.Respuestas;

namespace BiblioTech.Api.Services.Interfaces;

public interface IPrestamoService
{
    Task<Resultado<List<PrestamoDto>>> ListarAsync(string? estado, int? socioId);
    Task<Resultado<PrestamoDto>> ObtenerAsync(int id);
    Task<Resultado<List<PrestamoVencidoDto>>> ListarVencidosAsync();
    Task<Resultado<PrestamoDto>> PrestarAsync(PrestamoCrearDto dto);
    Task<Resultado> DevolverAsync(int id);
}
