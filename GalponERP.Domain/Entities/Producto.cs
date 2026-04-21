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
    /// Puede ser cero para productos que no requieren pesaje (Medicamentos, Insumos).
    /// </summary>
    public decimal PesoUnitarioKg { get; private set; }

    /// <summary>
    /// Stock total acumulado en unidades (Bolsas, Frascos, etc.) según su UnidadMedida.
    /// </summary>
    public decimal StockActual { get; private set; }

    /// <summary>
    /// Stock total acumulado en Kilogramos (StockActual * PesoUnitarioKg).
    /// </summary>
    public decimal StockActualKg { get; private set; }

    /// <summary>
    /// Cantidad mínima en stock (en unidades) antes de generar una alerta.
    /// </summary>
    public decimal UmbralMinimo { get; private set; }

    /// <summary>
    /// Días de espera requeridos tras el consumo de este producto antes del sacrificio (Seguridad Alimentaria).
    /// </summary>
    public int PeriodoRetiroDias { get; private set; }

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
        decimal stockInicial = 0,
        int periodoRetiroDias = 0) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");
        
        if (pesoUnitarioKg < 0)
            throw new ArgumentException("El peso unitario en Kg no puede ser negativo.");

        if (periodoRetiroDias < 0)
            throw new ArgumentException("El periodo de retiro no puede ser negativo.");

        Nombre = nombre;
        CategoriaProductoId = categoriaProductoId;
        UnidadMedidaId = unidadMedidaId;
        PesoUnitarioKg = pesoUnitarioKg;
        UmbralMinimo = umbralMinimo;
        CostoUnitarioActual = costoUnitarioActual;
        StockActual = stockInicial;
        StockActualKg = stockInicial * pesoUnitarioKg;
        PeriodoRetiroDias = periodoRetiroDias;
    }

    // Constructor para EF Core
    private Producto() : base() { }

    public void Actualizar(
        string nombre, 
        Guid categoriaProductoId, 
        Guid unidadMedidaId, 
        decimal pesoUnitarioKg,
        decimal umbralMinimo,
        decimal stockInicial,
        int periodoRetiroDias = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        
        if (pesoUnitarioKg < 0)
            throw new ArgumentException("El peso unitario en Kg no puede ser negativo.");

        if (periodoRetiroDias < 0)
            throw new ArgumentException("El periodo de retiro no puede ser negativo.");
        
        Nombre = nombre;
        CategoriaProductoId = categoriaProductoId;
        UnidadMedidaId = unidadMedidaId;
        PesoUnitarioKg = pesoUnitarioKg;
        UmbralMinimo = umbralMinimo;
        PeriodoRetiroDias = periodoRetiroDias;

        // Al actualizar el peso unitario o el stock inicial, sincronizamos el stock en Kg
        SincronizarStockKg(stockInicial);
    }

    /// <summary>
    /// Actualiza el stock total según un movimiento.
    /// </summary>
    /// <param name="cantidad">Cantidad en unidades base del producto.</param>
    /// <param name="tipo">Tipo de movimiento (Entrada/Salida).</param>
    public void ActualizarStock(decimal cantidad, TipoMovimiento tipo)
    {
        decimal factor = (tipo == TipoMovimiento.Entrada || tipo == TipoMovimiento.AjusteEntrada || tipo == TipoMovimiento.Compra) ? 1 : -1;
        
        StockActual += cantidad * factor;
        if (StockActual < 0) StockActual = 0;

        StockActualKg = StockActual * PesoUnitarioKg;
        if (StockActualKg < 0) StockActualKg = 0; 
    }

    /// <summary>
    /// Sincroniza el stock en Kg basado en el stock actual en unidades.
    /// Útil para migraciones o correcciones de integridad.
    /// </summary>
    public void SincronizarStockKg(decimal stockActualUnidades)
    {
        StockActual = stockActualUnidades;
        StockActualKg = stockActualUnidades * PesoUnitarioKg;
        
        if (StockActual < 0) StockActual = 0;
        if (StockActualKg < 0) StockActualKg = 0;
    }

    /// <summary>
    /// Convierte una cantidad de unidades físicas (bolsas, frascos, etc.) a kilogramos.
    /// Basado en el PesoUnitarioKg actual del producto.
    /// </summary>
    public decimal CalcularKg(decimal unidades)
    {
        return Math.Round(unidades * PesoUnitarioKg, 6);
    }

    /// <summary>
    /// Convierte una cantidad de kilogramos a unidades físicas.
    /// Si el PesoUnitarioKg es 0, retorna 0 para evitar división por cero.
    /// </summary>
    public decimal CalcularUnidades(decimal kg)
    {
        if (PesoUnitarioKg == 0) return 0;
        return Math.Round(kg / PesoUnitarioKg, 6);
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
