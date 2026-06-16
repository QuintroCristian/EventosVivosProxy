using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces.Repositories;

public interface IVenueRepository
{
    Task<IEnumerable<Venue>> GetAllAsync();
}
