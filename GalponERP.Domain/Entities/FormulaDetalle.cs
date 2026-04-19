using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

public class FormulaDetalle : Entity
{
    public Guid FormulaId { get; private set; }
    public Formula Formula { get; private set; } = null!;
    public Guid ProductoId { get; private set; }
    public Producto Producto { get; private set; } = null!;
    public decimal CantidadPorBase { get; private set; }

    public FormulaDetalle(Guid id, Guid formulaId, Guid productoId, decimal cantidadPorBase) : base(id)
    {
        FormulaId = formulaId;
        ProductoId = productoId;
        CantidadPorBase = cantidadPorBase;
    }

    // Para EF Core
    private FormulaDetalle() : base() { }
}
