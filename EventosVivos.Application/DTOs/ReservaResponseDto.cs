using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.DTOs;

public class ReservaResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int EventoId { get; set; }
    public string EventoTitulo { get; set; } = string.Empty;
    public string NombreComprador { get; set; } = string.Empty;
    public string EmailComprador { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal MontoTotal { get; set; }
    public EstadoReserva Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Penalizada { get; set; }
}
