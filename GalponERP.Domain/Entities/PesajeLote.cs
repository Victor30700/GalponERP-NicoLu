using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa el registro de pesaje de una muestra de pollos de un lote para monitorear el crecimiento.
/// </summary>
public class PesajeLote : Entity
{
    public Guid LoteId { get; private set; }
    public DateTime Fecha { get; private set; }
    public decimal PesoPromedioGramos { get; private set; }
    public int CantidadMuestreada { get; private set; }
    public Guid UsuarioId { get; private set; }

    public PesajeLote(Guid id, Guid loteId, DateTime fecha, decimal pesoPromedioGramos, int cantidadMuestreada, Guid usuarioId) : base(id)
    {
        if (loteId == Guid.Empty)
            throw new ArgumentException("El ID del lote es obligatorio.");

        if (pesoPromedioGramos <= 0)
            throw new ArgumentException("El peso promedio debe ser mayor a cero.");

        if (cantidadMuestreada <= 0)
            throw new ArgumentException("La cantidad muestreada debe ser mayor a cero.");

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("El ID del usuario es requerido para auditoría.");

        LoteId = loteId;
        Fecha = fecha;
        PesoPromedioGramos = pesoPromedioGramos;
        CantidadMuestreada = cantidadMuestreada;
        UsuarioId = usuarioId;
    }

    // Constructor para EF Core
    private PesajeLote() : base() { }

    public void Actualizar(DateTime fecha, decimal pesoPromedioGramos, int cantidadMuestreada)
    {
        if (pesoPromedioGramos <= 0)
            throw new ArgumentException("El peso promedio debe ser mayor a cero.");

        if (cantidadMuestreada <= 0)
            throw new ArgumentException("La cantidad muestreada debe ser mayor a cero.");

        Fecha = fecha;
        PesoPromedioGramos = pesoPromedioGramos;
        CantidadMuestreada = cantidadMuestreada;
    }
}
