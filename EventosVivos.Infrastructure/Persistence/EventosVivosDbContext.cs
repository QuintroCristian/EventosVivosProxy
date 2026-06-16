using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Persistence;

public class EventosVivosDbContext : DbContext
{
    public EventosVivosDbContext(DbContextOptions<EventosVivosDbContext> options)
        : base(options)
    {
    }

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Venue> Venues => Set<Venue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            // Backing field para Estado calculado: se persiste el enum como int
            entity.Property<EstadoEvento>("_estadoBase")
                  .IsRequired()
                  .HasConversion<int>();
            entity.Ignore(e => e.Estado);
            entity.HasOne(e => e.Venue)
                  .WithMany(v => v.Eventos)
                  .HasForeignKey(e => e.VenueId);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Codigo).HasMaxLength(20);
            entity.Property(r => r.NombreComprador).IsRequired().HasMaxLength(200);
            entity.Property(r => r.EmailComprador).IsRequired().HasMaxLength(200);
            entity.HasOne(r => r.Evento)
                  .WithMany(e => e.Reservas)
                  .HasForeignKey(r => r.EventoId);
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(v => v.Direccion).HasMaxLength(500);
            entity.Property(v => v.Ciudad).HasMaxLength(100);
        });

        // Seed: Venues iniciales para pruebas
        modelBuilder.Entity<Venue>().HasData(
            new Venue(1, "Auditorio Central",200, "Bogotá"),
            new Venue(2, "Sala Norte",50, "Bogotá"),
            new Venue(3, "Arena Sur", 500, "Medellín")
        );
    }
}
