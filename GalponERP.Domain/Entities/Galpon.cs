using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Entidad que representa un Galpón físico.
/// </summary>
public class Galpon : Entity
{
    public string Nombre { get; private set; } = null!;
    public int Capacidad { get; private set; }
    public string Ubicacion { get; private set; } = null!;

    public Galpon(Guid id, string nombre, int capacidad, string ubicacion) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre)) 
            throw new ArgumentException("El nombre del galpón es requerido.");
            
        if (capacidad <= 0) 
            throw new ArgumentException("La capacidad del galpón debe ser un número positivo.");

        Nombre = nombre;
        Capacidad = capacidad;
        Ubicacion = ubicacion;
    }

    // Constructor para EF Core
    private Galpon() : base() { }

    public void Actualizar(string nombre, int capacidad, string ubicacion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del galpón es requerido.");

        if (capacidad <= 0)
            throw new ArgumentException("La capacidad del galpón debe ser un número positivo.");

        Nombre = nombre;
        Capacidad = capacidad;
        Ubicacion = ubicacion;
    }
}
