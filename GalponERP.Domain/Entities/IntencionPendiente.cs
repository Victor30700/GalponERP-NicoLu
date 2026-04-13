using GalponERP.Domain.Primitives;

namespace GalponERP.Domain.Entities;

public class IntencionPendiente : Entity
{
    public Guid ConversacionId { get; private set; }
    public string PluginNombre { get; private set; }
    public string FuncionNombre { get; private set; }
    public string ParametrosJson { get; private set; }
    public bool Procesada { get; private set; }

    private IntencionPendiente() 
    {
        PluginNombre = string.Empty;
        FuncionNombre = string.Empty;
        ParametrosJson = string.Empty;
    } // Para EF Core

    public IntencionPendiente(Guid id, Guid conversacionId, string pluginNombre, string funcionNombre, string parametrosJson)
        : base(id)
    {
        ConversacionId = conversacionId;
        PluginNombre = pluginNombre;
        FuncionNombre = funcionNombre;
        ParametrosJson = parametrosJson;
        Procesada = false;
    }

    public void MarcarComoProcesada()
    {
        Procesada = true;
        Desactivar(); // Soft delete después de procesar
    }
}
