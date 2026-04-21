using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa un lote físico o de fabricante de un producto específico, 
/// permitiendo rastrear fechas de vencimiento y asegurar trazabilidad sanitaria.
/// </summary>
public class InventarioLote : Entity
{
    public Guid ProductoId { get; private set; }
    public Producto Producto { get; private set; } = null!;
    public string CodigoLote { get; private set; } = null!; // El lote que viene impreso en el saco/frasco
    public DateTime FechaVencimiento { get; private set; }
    public decimal StockActual { get; private set; }
    public DateTime FechaIngreso { get; private set; }
    public bool Agotado => StockActual <= 0;

    public InventarioLote(Guid id, Guid productoId, string codigoLote, DateTime fechaVencimiento, decimal stockInicial, DateTime fechaIngreso) 
        : base(id)
    {
        if (productoId == Guid.Empty)
            throw new InventarioDomainException("El ID del producto es obligatorio.");

        if (string.IsNullOrWhiteSpace(codigoLote))
            throw new InventarioDomainException("El código de lote del fabricante es obligatorio.");

        if (stockInicial < 0)
            throw new InventarioDomainException("El stock inicial no puede ser negativo.");

        ProductoId = productoId;
        CodigoLote = codigoLote;
        FechaVencimiento = fechaVencimiento;
        StockActual = stockInicial;
        FechaIngreso = fechaIngreso;
    }

    public void ActualizarStock(decimal cantidad, TipoMovimiento tipo)
    {
        decimal factor = (tipo == TipoMovimiento.Entrada || tipo == TipoMovimiento.AjusteEntrada || tipo == TipoMovimiento.Compra) ? 1 : -1;
        
        StockActual += cantidad * factor;
        if (StockActual < 0) StockActual = 0;
    }

    public bool EstaVencido(DateTime fechaConsulta)
    {
        return fechaConsulta.Date > FechaVencimiento.Date;
    }

    // Constructor para EF Core
    private InventarioLote() : base() { }
}
