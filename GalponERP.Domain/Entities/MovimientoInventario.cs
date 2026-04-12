using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;
using GalponERP.Domain.ValueObjects;

namespace GalponERP.Domain.Entities;

public enum TipoMovimiento
{
    Entrada,
    Salida,
    AjusteEntrada,
    AjusteSalida,
    Compra // Nuevo tipo para compras formales
}

/// <summary>
/// Representa un movimiento (entrada o salida) de un producto en el inventario.
/// </summary>
public class MovimientoInventario : Entity
{
    public Guid ProductoId { get; private set; }
    public Guid? LoteId { get; private set; } // Puede ser nulo si no está asociado a un lote específico
    public decimal Cantidad { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? Justificacion { get; private set; }
    public Guid UsuarioId { get; private set; }
    
    /// <summary>
    /// Costo total de la operación (para Entradas/Compras).
    /// </summary>
    public Moneda? CostoTotal { get; private set; }

    public string? Proveedor { get; private set; }
    public Guid? CompraId { get; private set; }

    public MovimientoInventario(
        Guid id, 
        Guid productoId, 
        Guid? loteId, 
        decimal cantidad, 
        TipoMovimiento tipo, 
        DateTime fecha, 
        Guid usuarioId, 
        string? justificacion = null,
        Moneda? costoTotal = null,
        string? proveedor = null,
        Guid? compraId = null) 
        : base(id)
    {
        if (productoId == Guid.Empty)
            throw new InventarioDomainException("El ID del producto es obligatorio.");

        if (cantidad <= 0)
            throw new InventarioDomainException("La cantidad del movimiento debe ser mayor a cero.");

        if (usuarioId == Guid.Empty)
            throw new InventarioDomainException("El ID del usuario es obligatorio para auditoría.");

        ProductoId = productoId;
        LoteId = loteId;
        Cantidad = cantidad;
        Tipo = tipo;
        Fecha = fecha;
        Justificacion = justificacion;
        UsuarioId = usuarioId;
        CostoTotal = costoTotal;
        Proveedor = proveedor;
        CompraId = compraId;
    }

    // Constructor para EF Core
    private MovimientoInventario() : base() { }
}
