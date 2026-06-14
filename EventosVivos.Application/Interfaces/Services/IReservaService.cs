using EventosVivos.Application.DTOs;

namespace EventosVivos.Application.Interfaces.Services;

public interface IReservaService
{
    Task<IEnumerable<ReservaResponseDto>> GetAllAsync();
    Task<ReservaResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<ReservaResponseDto>> GetByEventoIdAsync(int eventoId);
    Task<ReservaResponseDto> CrearAsync(CrearReservaRequest request);
    Task<ReservaResponseDto> ConfirmarPagoAsync(int id);
    Task CancelarAsync(int id);
}
