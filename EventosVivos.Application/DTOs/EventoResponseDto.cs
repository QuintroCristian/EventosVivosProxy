using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.DTOs;

public class EventoResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public TipoEvento Tipo { get; set; }
    public EstadoEvento Estado { get; set; }
    public string VenueNombre { get; set; } = string.Empty;
    public string VenueCiudad { get; set; } = string.Empty;
    public int CapacidadMaxima { get; set; }
    public decimal Precio { get; set; }
}
