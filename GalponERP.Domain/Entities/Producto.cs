using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

public enum TipoProducto
{
    Alimento,
    Medicamento,
    Insumo,
    Otro
}

public enum UnidadMedida
{
    Kg,
    Unidad,
    Litro,
    Saco
}

/// <summary>
/// Entidad que representa los productos (alimento, vacunas, etc.) en inventario.
/// </summary>
public class Producto : Entity
{
    public string Nombre { get; private set; } = null!;
    public TipoProducto Tipo { get; private set; }
    public UnidadMedida UnidadMedida { get; private set; }

    public Producto(Guid id, string nombre, TipoProducto tipo, UnidadMedida unidadMedida) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        Nombre = nombre;
        Tipo = tipo;
        UnidadMedida = unidadMedida;
    }

    // Constructor para EF Core
    private Producto() : base() { }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nuevo nombre no puede estar vacío.");
        Nombre = nuevoNombre;
    }
}
