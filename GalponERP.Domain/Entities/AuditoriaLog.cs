using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Registra cualquier acción crítica (PUT/DELETE) para trazabilidad total.
/// </summary>
public class AuditoriaLog : Entity
{
    public Guid UsuarioId { get; private set; }
    public string Accion { get; private set; } = string.Empty; // Actualizar, Eliminar
    public string Entidad { get; private set; } = string.Empty; // Mortalidad, Pesaje, etc.
    public Guid EntidadId { get; private set; }
    public DateTime Fecha { get; private set; }
    public string DetallesJSON { get; private set; } = string.Empty;

    public AuditoriaLog(Guid id, Guid usuarioId, string accion, string entidad, Guid entidadId, string detallesJson) 
        : base(id)
    {
        UsuarioId = usuarioId;
        Accion = accion;
        Entidad = entidad;
        EntidadId = entidadId;
        Fecha = DateTime.UtcNow;
        DetallesJSON = detallesJson;
    }

    // Constructor para EF Core
    private AuditoriaLog() : base() { }
}
