using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Categoría de producto (Alimento, Medicamento, Insumo, etc.)
/// </summary>
public class CategoriaProducto : Entity
{
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public TipoCategoria Tipo { get; private set; }

    public CategoriaProducto(Guid id, string nombre, string? descripcion = null, TipoCategoria tipo = TipoCategoria.Otros) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        Nombre = nombre;
        Descripcion = descripcion;
        Tipo = tipo;
    }

    // Para EF Core
    private CategoriaProducto() : base() { }

    public void Actualizar(string nombre, string? descripcion, TipoCategoria tipo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.");

        Nombre = nombre;
        Descripcion = descripcion;
        Tipo = tipo;
    }
}
