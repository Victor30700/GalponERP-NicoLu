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
    public decimal PesoUnitarioKg { get; private set; }

    /// <summary>
    /// Stock total acumulado en Kilogramos.
    /// Se actualiza con cada entrada/salida de stock.
    /// </summary>
    public decimal StockActualKg { get; private set; }

    /// <summary>
    /// Cantidad mínima en stock antes de generar una alerta.
    /// </summary>
    public decimal UmbralMinimo { get; private set; }

    /// <summary>
    /// Costo unitario promedio ponderado (PPP) del producto.
    /// Se actualiza con cada compra formal.
    /// </summary>
    public decimal CostoUnitarioActual { get; private set; }

    public Producto(
        Guid id, 
        string nombre, 
        Guid categoriaProductoId, 
        Guid unidadMedidaId, 
        decimal pesoUnitarioKg,
        decimal umbralMinimo = 0,
        decimal costoUnitarioActual = 0,
        decimal stockInicialKg = 0) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");
        
        if (pesoUnitarioKg <= 0)
            throw new ArgumentException("El peso unitario en Kg debe ser mayor a cero.");

        Nombre = nombre;
        CategoriaProductoId = categoriaProductoId;
        UnidadMedidaId = unidadMedidaId;
        PesoUnitarioKg = pesoUnitarioKg;
        UmbralMinimo = umbralMinimo;
        CostoUnitarioActual = costoUnitarioActual;
        StockActualKg = stockInicialKg;
    }

    // Constructor para EF Core
    private Producto() : base() { }

    public void Actualizar(
        string nombre, 
        Guid categoriaProductoId, 
        Guid unidadMedidaId, 
        decimal pesoUnitarioKg,
        decimal umbralMinimo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        
        if (pesoUnitarioKg <= 0)
            throw new ArgumentException("El peso unitario en Kg debe ser mayor a cero.");
        
        Nombre = nombre;
        CategoriaProductoId = categoriaProductoId;
        UnidadMedidaId = unidadMedidaId;
        PesoUnitarioKg = pesoUnitarioKg;
        UmbralMinimo = umbralMinimo;
    }

    /// <summary>
    /// Actualiza el stock total en Kg según un movimiento.
    /// </summary>
    public void ActualizarStock(decimal cantidad, TipoMovimiento tipo)
    {
        decimal factor = (tipo == TipoMovimiento.Entrada || tipo == TipoMovimiento.AjusteEntrada || tipo == TipoMovimiento.Compra) ? 1 : -1;
        StockActualKg += (cantidad * PesoUnitarioKg) * factor;

        if (StockActualKg < 0) StockActualKg = 0; // Evitar stock negativo en Kg por ajustes
    }

    /// <summary>
    /// Sincroniza el stock en Kg basado en el stock actual en unidades.
    /// Útil para migraciones o correcciones de integridad.
    /// </summary>
    public void SincronizarStockKg(decimal stockActualUnidades)
    {
        StockActualKg = stockActualUnidades * PesoUnitarioKg;
        if (StockActualKg < 0) StockActualKg = 0;
    }

    /// <summary>
    /// Actualiza el costo unitario del producto utilizando el algoritmo de Precio Promedio Ponderado.
    /// PPP = ((StockActual * CostoActual) + (CantidadComprada * PrecioCompra)) / (StockActual + CantidadComprada)
    /// </summary>
    public void RecalcularCostoPPP(decimal stockActual, decimal cantidadComprada, decimal precioCompra)
    {
        if (cantidadComprada <= 0) return;

        decimal stockNuevo = stockActual + cantidadComprada;
        
        if (stockNuevo <= 0)
        {
            // Si el stock nuevo es cero o negativo (ajuste extraño), mantenemos el costo de la compra o el actual
            CostoUnitarioActual = precioCompra > 0 ? precioCompra : CostoUnitarioActual;
            return;
        }

        decimal valorTotalAnterior = stockActual * CostoUnitarioActual;
        decimal valorCompraNueva = cantidadComprada * precioCompra;
        
        CostoUnitarioActual = (valorTotalAnterior + valorCompraNueva) / stockNuevo;
    }
}
