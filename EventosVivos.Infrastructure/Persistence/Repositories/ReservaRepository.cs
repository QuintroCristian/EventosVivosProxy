using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Interfaces.Repositories;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Persistence.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly EventosVivosDbContext _context;

    public ReservaRepository(EventosVivosDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reserva>> GetAllAsync() =>
        await _context.Reservas.Include(r => r.Evento).ToListAsync();

    public async Task<Reserva?> GetByIdAsync(int id) =>
        await _context.Reservas.Include(r => r.Evento).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Reserva>> GetByEventoIdAsync(int eventoId) =>
        await _context.Reservas.Include(r => r.Evento).Where(r => r.EventoId == eventoId).ToListAsync();

    // Suma las entradas activas (no canceladas sin penalización) para validar capacidad
    public async Task<int> GetTotalReservadasAsync(int eventoId) =>
        await _context.Reservas
            .Where(r => r.EventoId == eventoId
                     && r.Estado != EstadoReserva.Cancelada)
            .SumAsync(r => r.Cantidad);

    public async Task<Reserva> AddAsync(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();
        return reserva;
    }

    public async Task<Reserva> UpdateAsync(Reserva reserva)
    {
        _context.Reservas.Update(reserva);
        await _context.SaveChangesAsync();
        return reserva;
    }

    public async Task DeleteAsync(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva is not null)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
        }
    }
}
