using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Persistence.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly EventosVivosDbContext _context;

    public VenueRepository(EventosVivosDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Venue>> GetAllAsync() =>
        await _context.Venues.ToListAsync();
}
