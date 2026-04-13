using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa un mensaje individual dentro de una conversación de chat.
/// </summary>
public class MensajeChat : Entity
{
    public Guid ConversacionId { get; private set; }
    public string Rol { get; private set; } = string.Empty; // user, assistant, system
    public string Contenido { get; private set; } = string.Empty;
    public DateTime Fecha { get; private set; }

    // Propiedad de navegación
    public Conversacion? Conversacion { get; private set; }

    public MensajeChat(Guid id, Guid conversacionId, string rol, string contenido) : base(id)
    {
        ConversacionId = conversacionId;
        Rol = rol;
        Contenido = contenido;
        Fecha = DateTime.UtcNow;
    }

    // Constructor para EF Core
    private MensajeChat() : base() { }
}
