using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Representa una sesión de chat entre un usuario y el asistente de IA.
/// </summary>
public class Conversacion : Entity
{
    public Guid UsuarioId { get; private set; }
    public string Titulo { get; private set; } = "Nueva conversación";
    public DateTime FechaInicio { get; private set; }
    public string Estado { get; private set; } = "Activa";
    public string? ResumenActual { get; private set; }
    public int UltimoIndiceMensajeResumido { get; private set; } = 0;

    // Propiedad de navegación
    public ICollection<MensajeChat> Mensajes { get; private set; } = new List<MensajeChat>();

    public Conversacion(Guid id, Guid usuarioId, string? titulo = null) : base(id)
    {
        UsuarioId = usuarioId;
        Titulo = string.IsNullOrWhiteSpace(titulo) ? $"Chat - {DateTime.Now:dd/MM/yyyy HH:mm}" : titulo;
        FechaInicio = DateTime.UtcNow;
    }

    public void ActualizarTitulo(string nuevoTitulo)
    {
        if (!string.IsNullOrWhiteSpace(nuevoTitulo))
            Titulo = nuevoTitulo;
    }

    public void ActualizarResumen(string nuevoResumen, int ultimoIndice)
    {
        ResumenActual = nuevoResumen;
        UltimoIndiceMensajeResumido = ultimoIndice;
    }

    // Constructor para EF Core
    private Conversacion() : base() { }

    public void Archivar() => Estado = "Archivada";
}
