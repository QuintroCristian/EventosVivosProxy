using EventosVivos.Domain.Enums;

namespace EventosVivos.Domain.Entities;

public class Reserva
{
    public int Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public int EventoId { get; private set; }
    public Evento? Evento { get; private set; }
    public int Cantidad { get; private set; }
    public string NombreComprador { get; private set; } = string.Empty;
    public string EmailComprador { get; private set; } = string.Empty;
    public EstadoReserva Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaCancelacion { get; private set; }

    // RN-07: indica si la cancelación fue con penalización
    public bool Penalizada { get; private set; }

    protected Reserva() { }

    // Factory method: valida reglas dependientes del Evento (RN-04, RN-05)
    public static Reserva Crear(Evento evento, int cantidad, string nombreComprador, string emailComprador)
    {
        if (cantidad < 1)
            throw new ArgumentException("La cantidad debe ser 1 o más.");

        var horasParaInicio = (evento.FechaInicio - DateTime.UtcNow).TotalHours;

        // RN-04: No se permiten reservas faltando menos de 1 hora
        if (horasParaInicio < 1)
            throw new InvalidOperationException("No se permiten reservas faltando menos de 1 hora para el evento.");

        // RN-05: A menos de 24 horas, máximo 5 entradas
        if (horasParaInicio < 24 && cantidad > 5)
            throw new InvalidOperationException("A menos de 24 horas del evento, solo se permiten 5 entradas por transacción.");

        // RN-05: Para eventos de más de $100, máximo 10 entradas
        if (evento.Precio > 100 && cantidad > 10)
            throw new InvalidOperationException("Para eventos de más de $100, el máximo es 10 entradas por transacción.");

        return new Reserva
        {
            EventoId = evento.Id,
            Cantidad = cantidad,
            NombreComprador = nombreComprador,
            EmailComprador = emailComprador,
            Estado = EstadoReserva.PendientePago,
            FechaCreacion = DateTime.UtcNow,
            Penalizada = false
        };
    }

    // RF-04: Confirmar pago; el código EV-{6dígitos} se genera desde el servicio
    public void ConfirmarPago(string codigoReserva)
    {
        if (Estado == EstadoReserva.Confirmada)
            throw new InvalidOperationException("La reserva ya está confirmada.");

        if (Estado == EstadoReserva.Cancelada)
            throw new InvalidOperationException("No se puede confirmar una reserva cancelada.");

        Codigo = codigoReserva;
        Estado = EstadoReserva.Confirmada;
    }

    // RF-05 y RN-07: Cancelar reserva; aplica penalización si faltan menos de 48 horas
    public void Cancelar(Evento eventoRelacionado)
    {
        if (Estado == EstadoReserva.Cancelada)
            throw new InvalidOperationException("La reserva ya se encuentra cancelada.");

        FechaCancelacion = DateTime.UtcNow;
        Estado = EstadoReserva.Cancelada;

        // RN-07: Si faltan menos de 48 horas, la cancelación es penalizada (no libera cupo)
        var horasParaInicio = (eventoRelacionado.FechaInicio - DateTime.UtcNow).TotalHours;
        if (horasParaInicio < 48)
            Penalizada = true;
    }
}
