using Microsoft.EntityFrameworkCore.ChangeTracking;
using GalponERP.Domain.Entities;
using System.Text.Json;

namespace GalponERP.Infrastructure.Persistence;

public class AuditEntry
{
    public EntityEntry Entry { get; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = "Sistema";
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();

    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public AuditoriaLog ToAudit()
    {
        var diff = new
        {
            before = OldValues.Count > 0 ? OldValues : null,
            after = NewValues.Count > 0 ? NewValues : null
        };

        return new AuditoriaLog(
            Guid.NewGuid(),
            UsuarioId,
            UsuarioNombre,
            Action,
            TableName,
            TableName,
            (Guid)(KeyValues.Values.FirstOrDefault() ?? Guid.Empty),
            $"Entity Diff for {TableName} ({Action})",
            JsonSerializer.Serialize(diff)
        );
    }
}
