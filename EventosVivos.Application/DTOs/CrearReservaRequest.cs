namespace EventosVivos.Application.DTOs;

public class CrearReservaRequest
{
    public int EventoId { get; set; }
    public string NombreComprador { get; set; } = string.Empty;
    public string EmailComprador { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
