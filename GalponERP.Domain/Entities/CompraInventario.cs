using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa una compra de insumos/productos a un proveedor.
/// </summary>
public class CompraInventario : Entity
{
    public Guid ProveedorId { get; private set; }
    public DateTime Fecha { get; private set; }
    public Moneda Total { get; private set; } = null!;
    public Moneda TotalPagado { get; private set; } = null!;
    public EstadoPago EstadoPago { get; private set; }
    public Guid UsuarioIdRegistro { get; private set; }
    public string? Nota { get; private set; }

    private readonly List<PagoCompra> _pagos = new();
    public IReadOnlyCollection<PagoCompra> Pagos => _pagos.AsReadOnly();

    public Moneda SaldoPendiente => Total - TotalPagado;

    public CompraInventario(Guid id, Guid proveedorId, DateTime fecha, Moneda total, Moneda totalPagado, Guid usuarioIdRegistro, string? nota = null) 
        : base(id)
    {
        if (proveedorId == Guid.Empty)
            throw new ArgumentException("El ID del proveedor es requerido.", nameof(proveedorId));

        if (fecha == default)
            throw new ArgumentException("La fecha es inválida.", nameof(fecha));

        if (total == null)
            throw new ArgumentNullException(nameof(total));

        if (totalPagado == null)
            throw new ArgumentNullException(nameof(totalPagado));

        if (totalPagado.Monto > total.Monto)
            throw new InvalidOperationException("El monto pagado no puede ser mayor al total de la compra.");

        if (usuarioIdRegistro == Guid.Empty)
            throw new ArgumentException("El ID del usuario es requerido.", nameof(usuarioIdRegistro));

        ProveedorId = proveedorId;
        Fecha = fecha;
        Total = total;
        TotalPagado = totalPagado;
        UsuarioIdRegistro = usuarioIdRegistro;
        Nota = nota;

        ActualizarEstadoPago();
    }

    private void ActualizarEstadoPago()
    {
        if (TotalPagado.Monto >= Total.Monto)
        {
            EstadoPago = EstadoPago.Pagado;
        }
        else if (TotalPagado.Monto > 0)
        {
            EstadoPago = EstadoPago.Parcial;
        }
        else
        {
            EstadoPago = EstadoPago.Pendiente;
        }
    }

    public void RegistrarPago(Guid id, Moneda monto, DateTime fechaPago, MetodoPago metodoPago, Guid usuarioId)
    {
        if (monto.Monto > SaldoPendiente.Monto)
            throw new InventarioDomainException($"El monto del pago ({monto.Monto}) excede el saldo pendiente ({SaldoPendiente.Monto}).");

        var pago = new PagoCompra(id, Id, monto, fechaPago, metodoPago, usuarioId);
        _pagos.Add(pago);

        TotalPagado = new Moneda(TotalPagado.Monto + monto.Monto);
        ActualizarEstadoPago();
    }

    public void AnularPago(Guid pagoId)
    {
        var pago = _pagos.FirstOrDefault(p => p.Id == pagoId && p.IsActive);
        if (pago == null)
            throw new InventarioDomainException("El pago no existe o ya fue anulado.");

        pago.Eliminar(); // Soft delete
        TotalPagado = new Moneda(TotalPagado.Monto - pago.Monto.Monto);
        ActualizarEstadoPago();
    }

    // Constructor para EF Core
    private CompraInventario() : base() { }
}
