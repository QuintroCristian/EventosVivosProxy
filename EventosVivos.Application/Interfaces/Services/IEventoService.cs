using EventosVivos.Application.DTOs;

namespace EventosVivos.Application.Interfaces.Services;

public interface IEventoService
{
    Task<IEnumerable<EventoResponseDto>> GetAllAsync();
    Task<EventoResponseDto?> GetByIdAsync(int id);
    Task<EventoResponseDto> CrearAsync(EventoResponseDto dto);
    Task<EventoResponseDto> ActualizarAsync(int id, EventoResponseDto dto);
    Task EliminarAsync(int id);
    Task<IEnumerable<VenueDto>> GetVenuesAsync();
}
