using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Registra cualquier acción crítica (PUT/DELETE) para trazabilidad total.
/// </summary>
public class AuditoriaLog : Entity
{
    public Guid UsuarioId { get; private set; }
    public string UsuarioNombre { get; private set; } = string.Empty;
    public string Accion { get; private set; } = string.Empty; // Actualizar, Eliminar
    public string Entidad { get; private set; } = string.Empty; // Mortalidad, Pesaje, etc.
    public string EntidadNombre { get; private set; } = string.Empty;
    public Guid EntidadId { get; private set; }
    public DateTime Fecha { get; private set; }
    public string Detalles { get; private set; } = string.Empty;
    public string DetallesJSON { get; private set; } = string.Empty;

    public AuditoriaLog(Guid id, Guid usuarioId, string usuarioNombre, string accion, string entidad, string entidadNombre, Guid entidadId, string detalles, string detallesJson) 
        : base(id)
    {
        UsuarioId = usuarioId;
        UsuarioNombre = usuarioNombre;
        Accion = accion;
        Entidad = entidad;
        EntidadNombre = entidadNombre;
        EntidadId = entidadId;
        Fecha = DateTime.UtcNow;
        Detalles = detalles;
        DetallesJSON = detallesJson;
    }

    // Constructor para EF Core
    private AuditoriaLog() : base() { }
}
