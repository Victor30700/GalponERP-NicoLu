namespace GalponERP.Domain.Primitives;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Implementa el concepto de identidad y Soft Delete.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected init; }
    public bool IsActive { get; protected set; } = true;

    // Propiedades de Auditoría
    public DateTime FechaCreacion { get; private set; }
    public Guid? UsuarioCreacionId { get; private set; }
    public DateTime? FechaModificacion { get; private set; }
    public Guid? UsuarioModificacionId { get; private set; }

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

    public void SetAuditoriaCreacion(DateTime fecha, Guid usuarioId)
    {
        FechaCreacion = fecha;
        UsuarioCreacionId = usuarioId;
    }

    public void SetAuditoriaModificacion(DateTime fecha, Guid usuarioId)
    {
        FechaModificacion = fecha;
        UsuarioModificacionId = usuarioId;
    }

    public void Desactivar() => IsActive = false;
    public void Activar() => IsActive = true;
    public void Eliminar() => IsActive = false;
}
