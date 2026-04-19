using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

public class Formula : Entity
{
    public string Nombre { get; private set; } = null!;
    public string Etapa { get; private set; } = null!; // Ej. Iniciación, Crecimiento, Engorde
    public decimal CantidadBase { get; private set; } // Ej. 100 Kg
    
    private readonly List<FormulaDetalle> _detalles = new();
    public IReadOnlyCollection<FormulaDetalle> Detalles => _detalles.AsReadOnly();

    public Formula(Guid id, string nombre, string etapa, decimal cantidadBase) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new FormulaDomainException("El nombre de la fórmula es obligatorio.");
        
        if (string.IsNullOrWhiteSpace(etapa))
            throw new FormulaDomainException("La etapa es obligatoria.");

        if (cantidadBase <= 0)
            throw new FormulaDomainException("La cantidad base debe ser mayor a cero.");

        Nombre = nombre;
        Etapa = etapa;
        CantidadBase = cantidadBase;
    }

    // Para EF Core
    private Formula() : base() { }

    public void Actualizar(string nombre, string etapa, decimal cantidadBase)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new FormulaDomainException("El nombre de la fórmula es obligatorio.");
        
        if (string.IsNullOrWhiteSpace(etapa))
            throw new FormulaDomainException("La etapa es obligatoria.");

        if (cantidadBase <= 0)
            throw new FormulaDomainException("La cantidad base debe ser mayor a cero.");

        Nombre = nombre;
        Etapa = etapa;
        CantidadBase = cantidadBase;
    }

    public void AgregarDetalle(Guid productoId, decimal cantidadPorBase)
    {
        if (productoId == Guid.Empty)
            throw new FormulaDomainException("El ID del producto es obligatorio.");

        if (cantidadPorBase <= 0)
            throw new FormulaDomainException("La cantidad del ingrediente debe ser mayor a cero.");

        if (_detalles.Any(d => d.ProductoId == productoId))
            throw new FormulaDomainException("El producto ya existe en los detalles de la fórmula.");

        _detalles.Add(new FormulaDetalle(Guid.NewGuid(), Id, productoId, cantidadPorBase));
    }

    public void LimpiarDetalles() => _detalles.Clear();
}
