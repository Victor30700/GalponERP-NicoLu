using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Registra las bajas ocurridas en un lote en una fecha específica.
/// </summary>
public class MortalidadDiaria : Entity
{
    public Guid LoteId { get; private set; }
    public DateTime Fecha { get; private set; }
    public int CantidadBajas { get; private set; }
    public string Causa { get; private set; } = string.Empty;
    public Guid UsuarioId { get; private set; }

    public MortalidadDiaria(Guid id, Guid loteId, DateTime fecha, int cantidadBajas, string causa, Guid usuarioId) 
        : base(id)
    {
        if (loteId == Guid.Empty)
            throw new LoteDomainException("El ID del lote es obligatorio.");

        if (cantidadBajas <= 0)
            throw new LoteDomainException("La cantidad de bajas diarias debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(causa))
            throw new LoteDomainException("La causa de la baja es obligatoria.");

        if (usuarioId == Guid.Empty)
            throw new LoteDomainException("El ID del usuario es obligatorio para auditoría.");

        LoteId = loteId;
        Fecha = fecha;
        CantidadBajas = cantidadBajas;
        Causa = causa;
        UsuarioId = usuarioId;
    }

    public void Actualizar(DateTime fecha, int cantidadBajas, string causa)
    {
        if (cantidadBajas <= 0)
            throw new LoteDomainException("La cantidad de bajas diarias debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(causa))
            throw new LoteDomainException("La causa de la baja es obligatoria.");

        Fecha = fecha;
        CantidadBajas = cantidadBajas;
        Causa = causa;
    }

    // Constructor para EF Core
    private MortalidadDiaria() : base() { }
}
