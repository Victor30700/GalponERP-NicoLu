using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa un pago o amortización realizado a una compra de inventario específica.
/// </summary>
public class PagoCompra : Entity
{
    public Guid CompraId { get; private set; }
    public Moneda Monto { get; private set; } = null!;
    public DateTime FechaPago { get; private set; }
    public MetodoPago MetodoPago { get; private set; }
    public Guid UsuarioId { get; private set; }

    public PagoCompra(Guid id, Guid compraId, Moneda monto, DateTime fechaPago, MetodoPago metodoPago, Guid usuarioId) 
        : base(id)
    {
        if (compraId == Guid.Empty)
            throw new ArgumentException("El ID de la compra es requerido.", nameof(compraId));

        if (monto == null || monto.Monto <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor a cero.", nameof(monto));

        if (fechaPago == default)
            throw new ArgumentException("La fecha del pago es inválida.", nameof(fechaPago));

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("El ID del usuario es requerido para auditoría.", nameof(usuarioId));

        CompraId = compraId;
        Monto = monto;
        FechaPago = fechaPago;
        MetodoPago = metodoPago;
        UsuarioId = usuarioId;
    }

    // Constructor para EF Core
    private PagoCompra() : base() { }
}
