using EventosVivos.Domain.Enums;

namespace EventosVivos.Domain.Entities;

public class Evento
{
    public int Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int VenueId { get; private set; }
    public Venue? Venue { get; private set; }
    public int CapacidadMaxima { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public decimal Precio { get; private set; }
    public TipoEvento Tipo { get; private set; }

    private EstadoEvento _estadoBase;

    // RN-06: Estado calculado; si ya pasó su hora de fin y estaba Activo, se marca Completado
    public EstadoEvento Estado =>
        _estadoBase == EstadoEvento.Activo && DateTime.UtcNow > FechaFin
            ? EstadoEvento.Completado
            : _estadoBase;

    public ICollection<Reserva> Reservas { get; private set; } = new List<Reserva>();

    protected Evento() { }

    public Evento(string titulo, string descripcion, Venue venue, int capacidadMaxima,
                  DateTime fechaInicio, DateTime fechaFin, decimal precio, TipoEvento tipo)
    {
        if (string.IsNullOrWhiteSpace(titulo) || titulo.Length < 5 || titulo.Length > 100)
            throw new ArgumentException("El título debe tener entre 5 y 100 caracteres.");

        if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length < 10 || descripcion.Length > 500)
            throw new ArgumentException("La descripción debe tener entre 10 y 500 caracteres.");

        if (fechaInicio <= DateTime.UtcNow)
            throw new ArgumentException("La fecha de inicio debe ser en el futuro.");

        if (fechaFin <= fechaInicio)
            throw new ArgumentException("La fecha de fin debe ser posterior a la de inicio.");

        if (precio <= 0)
            throw new ArgumentException("El precio debe ser un valor positivo.");

        // RN-01: Capacidad del evento no puede exceder la del venue
        if (capacidadMaxima > venue.Capacidad)
            throw new InvalidOperationException($"La capacidad del evento ({capacidadMaxima}) no puede exceder la del venue ({venue.Capacidad}).");

        // RN-03: Eventos en fin de semana no pueden iniciar después de las 22:00
        if (fechaInicio.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            if (fechaInicio.TimeOfDay > new TimeSpan(22, 0, 0))
                throw new InvalidOperationException("Los eventos en fin de semana no pueden iniciar después de las 22:00.");
        }

        Titulo = titulo;
        Descripcion = descripcion;
        VenueId = venue.Id;
        CapacidadMaxima = capacidadMaxima;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Precio = precio;
        Tipo = tipo;
        _estadoBase = EstadoEvento.Activo;
    }

    public void Cancelar()
    {
        if (Estado == EstadoEvento.Completado)
            throw new InvalidOperationException("No se puede cancelar un evento completado.");

        _estadoBase = EstadoEvento.Cancelado;
    }
}
