using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Entidad que representa los productos (alimento, vacunas, etc.) en inventario.
/// </summary>
public class Producto : Entity
{
    public string Nombre { get; private set; } = null!;
    public Guid CategoriaProductoId { get; private set; }
    public CategoriaProducto Categoria { get; private set; } = null!;
    public Guid UnidadMedidaId { get; private set; }
    public UnidadMedida Unidad { get; private set; } = null!;
    
    /// <summary>
    /// Multiplicador para convertir la unidad de medida a Kilogramos.
    /// Crucial para cálculos de FCR (Food Conversion Ratio).
    /// </summary>
    public decimal EquivalenciaEnKg { get; private set; }

    /// <summary>
    /// Cantidad mínima en stock antes de generar una alerta.
    /// </summary>
    public decimal UmbralMinimo { get; private set; }

    public Producto(
        Guid id, 
        string nombre, 
        Guid categoriaProductoId, 
        Guid unidadMedidaId, 
        decimal equivalenciaEnKg,
        decimal umbralMinimo = 0) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");
        
        if (equivalenciaEnKg <= 0)
            throw new ArgumentException("La equivalencia en Kg debe ser mayor a cero.");

        Nombre = nombre;
        CategoriaProductoId = categoriaProductoId;
        UnidadMedidaId = unidadMedidaId;
        EquivalenciaEnKg = equivalenciaEnKg;
        UmbralMinimo = umbralMinimo;
    }

    // Constructor para EF Core
    private Producto() : base() { }

    public void Actualizar(
        string nombre, 
        Guid categoriaProductoId, 
        Guid unidadMedidaId, 
        decimal equivalenciaEnKg,
        decimal umbralMinimo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        
        if (equivalenciaEnKg <= 0)
            throw new ArgumentException("La equivalencia en Kg debe ser mayor a cero.");
        
        Nombre = nombre;
        CategoriaProductoId = categoriaProductoId;
        UnidadMedidaId = unidadMedidaId;
        EquivalenciaEnKg = equivalenciaEnKg;
        UmbralMinimo = umbralMinimo;
    }
}
