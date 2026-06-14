using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces.Services;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Interfaces.Repositories;

namespace EventosVivos.Application.UseCases.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IEventoRepository _eventoRepository;

    public ReservaService(IReservaRepository reservaRepository, IEventoRepository eventoRepository)
    {
        _reservaRepository = reservaRepository;
        _eventoRepository = eventoRepository;
    }

    public async Task<IEnumerable<ReservaResponseDto>> GetAllAsync()
    {
        var reservas = await _reservaRepository.GetAllAsync();
        return reservas.Select(MapToDto);
    }

    public async Task<ReservaResponseDto?> GetByIdAsync(int id)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id);
        return reserva is null ? null : MapToDto(reserva);
    }

    public async Task<IEnumerable<ReservaResponseDto>> GetByEventoIdAsync(int eventoId)
    {
        var reservas = await _reservaRepository.GetByEventoIdAsync(eventoId);
        return reservas.Select(MapToDto);
    }

    public async Task<ReservaResponseDto> CrearAsync(CrearReservaRequest request)
    {
        var evento = await _eventoRepository.GetByIdAsync(request.EventoId)
            ?? throw new InvalidOperationException($"Evento con ID {request.EventoId} no encontrado.");

        // RN-02: Validar que haya cupo disponible (sin contar canceladas penalizadas)
        var totalReservadas = await _reservaRepository.GetTotalReservadasAsync(request.EventoId);
        if (totalReservadas + request.Cantidad > evento.CapacidadMaxima)
            throw new InvalidOperationException("No hay suficiente capacidad disponible en el evento.");

        var reserva = Reserva.Crear(evento, request.Cantidad, request.NombreComprador, request.EmailComprador);

        var creada = await _reservaRepository.AddAsync(reserva);
        creada = await _reservaRepository.GetByIdAsync(creada.Id) ?? creada;
        return MapToDto(creada);
    }

    public async Task<ReservaResponseDto> ConfirmarPagoAsync(int id)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Reserva con ID {id} no encontrada.");

        var codigo = $"EV-{Random.Shared.Next(100000, 999999)}";
        reserva.ConfirmarPago(codigo);

        var actualizada = await _reservaRepository.UpdateAsync(reserva);
        return MapToDto(actualizada);
    }

    public async Task CancelarAsync(int id)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Reserva con ID {id} no encontrada.");

        var evento = await _eventoRepository.GetByIdAsync(reserva.EventoId)
            ?? throw new InvalidOperationException($"Evento con ID {reserva.EventoId} no encontrado.");

        reserva.Cancelar(evento);
        await _reservaRepository.UpdateAsync(reserva);
    }

    private static ReservaResponseDto MapToDto(Reserva reserva) => new()
    {
        Id = reserva.Id,
        Codigo = reserva.Codigo,
        EventoId = reserva.EventoId,
        EventoTitulo = reserva.Evento?.Titulo ?? string.Empty,
        NombreComprador = reserva.NombreComprador,
        EmailComprador = reserva.EmailComprador,
        Cantidad = reserva.Cantidad,
        MontoTotal = reserva.Cantidad * (reserva.Evento?.Precio ?? 0),
        Estado = reserva.Estado,
        FechaCreacion = reserva.FechaCreacion,
        Penalizada = reserva.Penalizada
    };
}
