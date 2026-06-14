using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces.Repositories;

public interface IEventoRepository
{
    Task<IEnumerable<Evento>> GetAllAsync();
    Task<Evento?> GetByIdAsync(int id);
    Task<Evento> AddAsync(Evento evento);
    Task<Evento> UpdateAsync(Evento evento);
    Task DeleteAsync(int id);
}
