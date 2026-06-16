namespace EventosVivos.Application.DTOs;

public class VenueDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public string Ciudad { get; set; } = string.Empty;
}
