using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa un gasto operativo asociado a un galpón o un lote específico (luz, agua, sueldos, etc.).
/// </summary>
public class GastoOperativo : Entity
{
    public Guid GalponId { get; private set; }
    public Guid? LoteId { get; private set; }
    public string Descripcion { get; private set; } = null!;
    public Moneda Monto { get; private set; } = null!;
    public DateTime Fecha { get; private set; }

    public GastoOperativo(Guid id, Guid galponId, Guid? loteId, string descripcion, Moneda monto, DateTime fecha) 
        : base(id)
    {
        if (galponId == Guid.Empty)
            throw new ArgumentException("El ID del galpón es requerido.", nameof(galponId));

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción del gasto es requerida.", nameof(descripcion));

        if (monto == null)
            throw new ArgumentNullException(nameof(monto));

        if (fecha == default)
            throw new ArgumentException("La fecha del gasto es inválida.", nameof(fecha));

        GalponId = galponId;
        LoteId = loteId;
        Descripcion = descripcion;
        Monto = monto;
        Fecha = fecha;
    }

    // Constructor para EF Core
    private GastoOperativo() : base() { }

    /// <summary>
    /// Actualiza la información del gasto operativo.
    /// </summary>
    public void Actualizar(string descripcion, Moneda monto, DateTime fecha, Guid? loteId = null)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción no puede ser vacía.", nameof(descripcion));

        if (monto == null)
            throw new ArgumentNullException(nameof(monto));

        if (fecha == default)
            throw new ArgumentException("La fecha es inválida.", nameof(fecha));

        Descripcion = descripcion;
        Monto = monto;
        Fecha = fecha;
        LoteId = loteId;
    }
}
