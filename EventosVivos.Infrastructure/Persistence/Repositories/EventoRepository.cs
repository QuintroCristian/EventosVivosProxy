using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Interfaces.Repositories;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Persistence.Repositories;

public class EventoRepository : IEventoRepository
{
    private readonly EventosVivosDbContext _context;

    public EventoRepository(EventosVivosDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Evento>> GetAllAsync() =>
        await _context.Eventos.Include(e => e.Venue).ToListAsync();

    public async Task<Evento?> GetByIdAsync(int id) =>
        await _context.Eventos.Include(e => e.Venue).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Evento> AddAsync(Evento evento)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
        return evento;
    }

    public async Task<Evento> UpdateAsync(Evento evento)
    {
        _context.Eventos.Update(evento);
        await _context.SaveChangesAsync();
        return evento;
    }

    public async Task DeleteAsync(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento is not null)
        {
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }
    }
}
