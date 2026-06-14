using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces.Services;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Interfaces.Repositories;

namespace EventosVivos.Application.UseCases.Services;

public class EventoService : IEventoService
{
    private readonly IEventoRepository _eventoRepository;

    public EventoService(IEventoRepository eventoRepository)
    {
        _eventoRepository = eventoRepository;
    }

    public async Task<IEnumerable<EventoResponseDto>> GetAllAsync()
    {
        var eventos = await _eventoRepository.GetAllAsync();
        return eventos.Select(MapToDto);
    }

    public async Task<EventoResponseDto?> GetByIdAsync(int id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        return evento is null ? null : MapToDto(evento);
    }

    public async Task<EventoResponseDto> CrearAsync(EventoResponseDto dto)
    {
        var evento = MapToEntity(dto);
        var creado = await _eventoRepository.AddAsync(evento);
        return MapToDto(creado);
    }


    //todo El parámetro `id` no se usa y `MapToEntity(dto)` no establece `Id`, por lo que el update puede terminar actualizando el registro equivocado o intentando insertar uno nuevo (dependiendo de cómo EF rastree la entidad). Solución típica: cargar la entidad existente por `id`, mutar sus campos y luego persistir; o asegurar que la entidad mapeada lleve el `Id` correcto.
    public async Task<EventoResponseDto> ActualizarAsync(int id, EventoResponseDto dto)
    {
        var evento = MapToEntity(dto);
        var actualizado = await _eventoRepository.UpdateAsync(evento);
        return MapToDto(actualizado);
    }

    public async Task EliminarAsync(int id) =>
        await _eventoRepository.DeleteAsync(id);

    private static EventoResponseDto MapToDto(Evento evento) => new()
    {
        Id = evento.Id,
        Titulo = evento.Titulo,
        Descripcion = evento.Descripcion,
        FechaInicio = evento.FechaInicio,
        FechaFin = evento.FechaFin,
        Tipo = evento.Tipo,
        Estado = evento.Estado,
        VenueNombre = evento.Venue?.Nombre ?? string.Empty,
        VenueCiudad = evento.Venue?.Ciudad ?? string.Empty,
        CapacidadMaxima = evento.CapacidadMaxima,
        Precio = evento.Precio
    };

    private static Evento MapToEntity(EventoResponseDto dto)
    {
        // Para actualizaciones, se necesita acceder al Venue real; aquí se construye
        // un Venue temporal con la capacidad del dto para satisfacer el constructor.
        // En un escenario real, el servicio debería recibir un DTO de creación separado.
        var venueTemp = new Venue(0, dto.VenueNombre, dto.CapacidadMaxima, dto.VenueCiudad);
        return new Evento(dto.Titulo, dto.Descripcion, venueTemp,
                          dto.CapacidadMaxima, dto.FechaInicio, dto.FechaFin,
                          dto.Precio, dto.Tipo);
    }
}
