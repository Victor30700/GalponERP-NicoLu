using System.ComponentModel;
using GalponERP.Application.Agentes.Confirmacion.Commands;
using GalponERP.Application.Agentes.Confirmacion.Queries;
using MediatR;
using Microsoft.SemanticKernel;

namespace GalponERP.Application.Agentes.Plugins;

public class ConfirmacionPlugin
{
    private readonly IMediator _mediator;

    public ConfirmacionPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Confirma y ejecuta la última acción que quedó pendiente de confirmación en esta conversación.")]
    public async Task<string> ConfirmarAccionPendiente(
        [Description("ID de la conversación actual")] Guid conversacionId)
    {
        try
        {
            var intencion = await _mediator.Send(new ObtenerIntencionPendienteQuery(conversacionId));

            if (intencion == null)
            {
                return "No hay ninguna acción pendiente de confirmación en este momento.";
            }

            // Aquí es donde se pone interesante. ¿Cómo re-ejecutamos el comando?
            // Una opción es que el Orquestador lo haga, o que este plugin conozca todos los plugins.
            // Pero para seguir el "Paso 3" del plan:
            // "Implementar lógica en el Orquestador para detectar respuestas afirmativas y re-ejecutar el comando pendiente."
            
            // Si el Orquestador lo hace, este plugin solo debería marcarlo como "Listo para ejecutar" o algo así.
            // Pero si el LLM llama a esta función, queremos que se ejecute.
            
            return $"Confirmado. Procedo a ejecutar: {intencion.FuncionNombre} del plugin {intencion.PluginNombre}. [EJECUTAR_PENDIENTE:{intencion.Id}]";
        }
        catch (Exception ex)
        {
            return $"Error al confirmar la acción pendiente: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Cancela la acción que estaba pendiente de confirmación.")]
    public async Task<string> CancelarAccionPendiente(
        [Description("ID de la conversación actual")] Guid conversacionId)
    {
        try
        {
            var intencion = await _mediator.Send(new ObtenerIntencionPendienteQuery(conversacionId));

            if (intencion == null)
            {
                return "No hay ninguna acción pendiente para cancelar.";
            }

            await _mediator.Send(new MarcarIntencionComoProcesadaCommand(intencion.Id));
            return "Acción cancelada exitosamente.";
        }
        catch (Exception ex)
        {
            return $"Error al cancelar la acción pendiente: {ex.Message}";
        }
    }
}
