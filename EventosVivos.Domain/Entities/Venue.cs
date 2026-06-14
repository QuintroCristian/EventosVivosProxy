namespace EventosVivos.Domain.Entities;

public class Venue
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public string Ciudad { get; private set; } = string.Empty;
    public int Capacidad { get; private set; }

    public ICollection<Evento> Eventos { get; private set; } = new List<Evento>();

    protected Venue() { }

    public Venue(int id, string nombre, int capacidad, string ciudad, string direccion = "")
    {
        Id = id;
        Nombre = nombre;
        Capacidad = capacidad;
        Ciudad = ciudad;
        Direccion = direccion;
    }
}
