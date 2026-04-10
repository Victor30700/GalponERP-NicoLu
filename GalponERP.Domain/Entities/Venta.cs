using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa una venta de pollos de un lote específico.
/// </summary>
public class Venta : Entity
{
    public Guid LoteId { get; private set; }
    public Guid ClienteId { get; private set; }
    public DateTime Fecha { get; private set; }
    public int CantidadPollos { get; private set; }
    public Moneda PrecioUnitario { get; private set; } = null!;
    public Moneda Total { get; private set; } = null!;

    public Venta(Guid id, Guid loteId, Guid clienteId, DateTime fecha, int cantidadPollos, Moneda precioUnitario) 
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

        if (precioUnitario == null)
            throw new ArgumentNullException(nameof(precioUnitario));

        LoteId = loteId;
        ClienteId = clienteId;
        Fecha = fecha;
        CantidadPollos = cantidadPollos;
        PrecioUnitario = precioUnitario;
        Total = precioUnitario * cantidadPollos;
    }

    // Constructor para EF Core
    private Venta() : base() { }
}
