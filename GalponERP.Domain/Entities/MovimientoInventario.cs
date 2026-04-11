using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

public enum TipoMovimiento
{
    Entrada,
    Salida,
    AjusteEntrada,
    AjusteSalida
}

/// <summary>
/// Representa un movimiento (entrada o salida) de un producto en el inventario.
/// </summary>
public class MovimientoInventario : Entity
{
    public Guid ProductoId { get; private set; }
    public Guid? LoteId { get; private set; } // Puede ser nulo si no está asociado a un lote específico (ej. compra inicial)
    public decimal Cantidad { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? Justificacion { get; private set; }

    public MovimientoInventario(Guid id, Guid productoId, Guid? loteId, decimal cantidad, TipoMovimiento tipo, DateTime fecha, string? justificacion = null) 
        : base(id)
    {
        if (productoId == Guid.Empty)
            throw new InventarioDomainException("El ID del producto es obligatorio.");

        if (cantidad <= 0)
            throw new InventarioDomainException("La cantidad del movimiento debe ser mayor a cero.");

        ProductoId = productoId;
        LoteId = loteId;
        Cantidad = cantidad;
        Tipo = tipo;
        Fecha = fecha;
        Justificacion = justificacion;
    }

    // Constructor para EF Core
    private MovimientoInventario() : base() { }
}
