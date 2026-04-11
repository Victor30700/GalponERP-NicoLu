namespace GalponERP.Domain.Primitives;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Implementa el concepto de identidad y Soft Delete.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected init; }
    public bool IsActive { get; protected set; } = true;

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("El ID no puede ser un Guid vacío.", nameof(id));
        }
        Id = id;
    }

    // Para EF Core
    protected Entity() { }

    public void Desactivar() => IsActive = false;
    public void Eliminar() => IsActive = false;
}
