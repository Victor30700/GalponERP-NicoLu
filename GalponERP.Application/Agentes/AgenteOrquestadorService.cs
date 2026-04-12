using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using GalponERP.Application.Agentes.Plugins;
using MediatR;
using GalponERP.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GalponERP.Application.Agentes;

public class AgenteOrquestadorService : IAgenteOrquestadorService
{
    private readonly Kernel _kernel;

    public AgenteOrquestadorService(Kernel kernel, IServiceProvider serviceProvider)
    {
        _kernel = kernel;
        
        // Registrar Plugins en el Kernel
        _kernel.Plugins.AddFromType<ProduccionPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<CatalogosPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<InventarioPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<FinanzasPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<GestionCatalogosPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<VentasPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<GestionLotesPlugin>(serviceProvider: serviceProvider);
    }

    public async Task<string> ProcesarMensajeAsync(string mensajeUsuario)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("Eres el Operador Maestro de GalponERP, un asistente virtual de élite para la gestión avícola. " +
                                     "Tu objetivo es tomar el control total del sistema y guiar al usuario de forma proactiva. " +
                                     $"La fecha y hora actual es: {DateTime.Now:f}. " +
                                     "REGLAS DE ORO: " +
                                     "1. PROACTIVIDAD: Si el usuario quiere registrar algo y falta información (ej. un galpón o producto), no te detengas; ejecuta la función con lo que tengas. El código C# te devolverá las opciones disponibles para que tú se las presentes al usuario. " +
                                     "2. FLUJOS COMPLETOS: Si un usuario quiere crear un producto y la categoría no existe, guíalo para crear la categoría primero o elegir una existente. " +
                                     "3. LENGUAJE NATURAL: NUNCA menciones GUIDs, IDs o tecnicismos. Habla siempre de nombres de galpones, productos y categorías. " +
                                     "4. TONO PROFESIONAL: Eres un consultor experto. Sé directo, eficiente y servicial.");
        chatHistory.AddUserMessage(mensajeUsuario);

        // Configuramos la invocación automática de funciones de forma genérica
        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        try
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings,
                _kernel);

            return result.Content ?? "No se generó respuesta del modelo.";
        }
        catch (Exception ex)
        {
            return $"Error al procesar el mensaje: {ex.Message}";
        }
    }
}
