using BiblioTech.Api.DTOs.Socios;
using BiblioTech2.Api.Respuestas;

namespace BiblioTech.Api.Services.Interfaces;

public interface ISocioService
{
    Task<Resultado<List<SocioDto>>> ListarAsync(bool? activos);
    Task<Resultado<SocioDto>> ObtenerAsync(int id);
    Task<Resultado<SocioDto>> CrearAsync(SocioGuardarDto dto);
    Task<Resultado> ActualizarAsync(int id, SocioGuardarDto dto);
    Task<Resultado> CambiarEstadoAsync(int id, bool activo);
}
