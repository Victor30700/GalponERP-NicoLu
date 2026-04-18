using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa una venta de pollos de un lote específico, basada en peso total vendido.
/// </summary>
public class Venta : Entity
{
    public Guid LoteId { get; private set; }
    public Guid ClienteId { get; private set; }
    public DateTime Fecha { get; private set; }
    public int CantidadPollos { get; private set; }
    public decimal PesoTotalVendido { get; private set; }
    public Moneda PrecioPorKilo { get; private set; } = null!;
    public Moneda Total { get; private set; } = null!;
    public Guid UsuarioId { get; private set; }
    public EstadoPago EstadoPago { get; private set; }

    private readonly List<PagoVenta> _pagos = new();
    public IReadOnlyCollection<PagoVenta> Pagos => _pagos.AsReadOnly();

    public Moneda SaldoPendiente => Total - new Moneda(_pagos.Where(p => p.IsActive).Sum(p => p.Monto.Monto));

    public Venta(Guid id, Guid loteId, Guid clienteId, DateTime fecha, int cantidadPollos, decimal pesoTotalVendido, Moneda precioPorKilo, Guid usuarioId, EstadoPago estadoPago = EstadoPago.Pendiente) 
        : base(id)
    {
        if (loteId == Guid.Empty)
            throw new ArgumentException("El ID del lote es requerido.", nameof(loteId));

        if (clienteId == Guid.Empty)
            throw new ArgumentException("El ID del cliente es requerido.", nameof(clienteId));

        if (fecha == default)
            throw new ArgumentException("La fecha es inválida.", nameof(fecha));

        if (cantidadPollos <= 0)
            throw new ArgumentException("La cantidad de pollos debe ser mayor a cero.", nameof(cantidadPollos));

        if (pesoTotalVendido <= 0)
            throw new ArgumentException("El peso total vendido debe ser mayor a cero.", nameof(pesoTotalVendido));

        if (precioPorKilo == null)
            throw new ArgumentNullException(nameof(precioPorKilo));

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("El ID del usuario es requerido para auditoría.", nameof(usuarioId));

        LoteId = loteId;
        ClienteId = clienteId;
        Fecha = fecha;
        CantidadPollos = cantidadPollos;
        PesoTotalVendido = pesoTotalVendido;
        PrecioPorKilo = precioPorKilo;
        Total = precioPorKilo * pesoTotalVendido;
        UsuarioId = usuarioId;
        EstadoPago = estadoPago;
    }

    public PagoVenta RegistrarPago(Guid id, Moneda monto, DateTime fechaPago, MetodoPago metodoPago, Guid usuarioId)
    {
        if (monto.Monto > SaldoPendiente.Monto)
            throw new InvalidOperationException($"El monto del pago ({monto}) no puede exceder el saldo pendiente ({SaldoPendiente}).");

        var pago = new PagoVenta(id, Id, monto, fechaPago, metodoPago, usuarioId);
        _pagos.Add(pago);

        ActualizarEstadoPagoSegunSaldos();
        
        return pago;
    }

    public void AnularPago(Guid pagoId, Guid usuarioId)
    {
        var pago = _pagos.FirstOrDefault(p => p.Id == pagoId);
        if (pago == null)
            throw new InvalidOperationException("El pago no existe en esta venta.");

        if (!pago.IsActive)
            return;

        pago.Desactivar();
        pago.SetAuditoriaModificacion(DateTime.UtcNow, usuarioId);

        ActualizarEstadoPagoSegunSaldos();
    }

    public void ActualizarDatos(int cantidadPollos, decimal pesoTotalVendido, Moneda precioPorKilo)
    {
        if (cantidadPollos <= 0)
            throw new ArgumentException("La cantidad de pollos debe ser mayor a cero.", nameof(cantidadPollos));

        if (pesoTotalVendido <= 0)
            throw new ArgumentException("El peso total vendido debe ser mayor a cero.", nameof(pesoTotalVendido));

        if (precioPorKilo == null)
            throw new ArgumentNullException(nameof(precioPorKilo));

        CantidadPollos = cantidadPollos;
        PesoTotalVendido = pesoTotalVendido;
        PrecioPorKilo = precioPorKilo;
        Total = precioPorKilo * pesoTotalVendido;

        ActualizarEstadoPagoSegunSaldos();
    }

    private void ActualizarEstadoPagoSegunSaldos()
    {
        decimal saldo = SaldoPendiente.Monto;
        decimal total = Total.Monto;

        if (saldo <= 0)
        {
            EstadoPago = EstadoPago.Pagado;
        }
        else if (saldo < total)
        {
            EstadoPago = EstadoPago.Parcial;
        }
        else
        {
            EstadoPago = EstadoPago.Pendiente;
        }
    }

    public void ActualizarEstadoPago(EstadoPago nuevoEstado)
    {
        EstadoPago = nuevoEstado;
    }

    // Constructor para EF Core
    private Venta() : base() { }
}
