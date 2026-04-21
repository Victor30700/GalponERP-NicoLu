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

        // Buscar si ya existe (incluso si está inactivo para reactivarlo si fuera necesario, 
        // pero por ahora lanzamos error si hay uno ACTIVO)
        if (_detalles.Any(d => d.ProductoId == productoId && d.IsActive))
            throw new FormulaDomainException("El producto ya existe en los detalles activos de la fórmula.");

        _detalles.Add(new FormulaDetalle(Guid.NewGuid(), Id, productoId, cantidadPorBase));
    }

    public void LimpiarDetalles() 
    {
        foreach(var detalle in _detalles.Where(d => d.IsActive))
        {
            detalle.Eliminar(); // Soft delete
        }
    }

    public void EliminarDetalle(Guid productoId)
    {
        var detalle = _detalles.FirstOrDefault(d => d.ProductoId == productoId && d.IsActive);
        if (detalle != null)
        {
            detalle.Eliminar(); // Soft delete en lugar de _detalles.Remove() para asegurar que EF lo vea como Modified -> IsActive=false
        }
    }

    public void ActualizarDetalle(Guid productoId, decimal nuevaCantidad)
    {
        var detalle = _detalles.FirstOrDefault(d => d.ProductoId == productoId && d.IsActive);
        if (detalle == null)
            throw new FormulaDomainException("El producto no existe o no está activo en los detalles de la fórmula.");

        if (nuevaCantidad <= 0)
            throw new FormulaDomainException("La cantidad debe ser mayor a cero.");

        detalle.ActualizarCantidad(nuevaCantidad);
    }
}
