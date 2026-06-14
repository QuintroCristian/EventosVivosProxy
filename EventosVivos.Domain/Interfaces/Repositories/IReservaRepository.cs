using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces.Repositories;

public interface IReservaRepository
{
    Task<IEnumerable<Reserva>> GetAllAsync();
    Task<Reserva?> GetByIdAsync(int id);
    Task<IEnumerable<Reserva>> GetByEventoIdAsync(int eventoId);
    Task<int> GetTotalReservadasAsync(int eventoId);
    Task<Reserva> AddAsync(Reserva reserva);
    Task<Reserva> UpdateAsync(Reserva reserva);
    Task DeleteAsync(int id);
}
