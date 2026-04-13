using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using GalponERP.Application.Agentes.Plugins;
using MediatR;
using GalponERP.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using GalponERP.Application.Agentes.Chat.Queries.ObtenerHistorialChat;
using GalponERP.Application.Agentes.Chat.Commands.CrearConversacion;
using GalponERP.Application.Agentes.Chat.Commands.GuardarMensaje;
using GalponERP.Application.Agentes.Chat.Commands.ActualizarResumen;
using GalponERP.Application.Dashboard.Queries;
using System.Text;
using System.Text.Json;

namespace GalponERP.Application.Agentes;

public class AgenteOrquestadorService : IAgenteOrquestadorService
{
    private readonly Kernel _kernel;
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUserContext;

    public AgenteOrquestadorService(Kernel kernel, IServiceProvider serviceProvider, IMediator mediator, ICurrentUserContext currentUserContext)
    {
        _kernel = kernel;
        _mediator = mediator;
        _currentUserContext = currentUserContext;
        
        // Registrar Plugins en el Kernel
        _kernel.Plugins.AddFromType<ProduccionPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<CatalogosPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<InventarioPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<FinanzasPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<GestionCatalogosPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<VentasPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<GestionLotesPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<ContextoPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<AuditoriaPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<SanidadPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<AbastecimientoPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<PesajesPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<ConfiguracionPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<ReportesPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<ConfirmacionPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<AdministracionPlugin>(serviceProvider: serviceProvider);
        _kernel.Plugins.AddFromType<AnalisisPlugin>(serviceProvider: serviceProvider);
    }

    public async Task<AgenteResponse> ProcesarMensajeAsync(string mensajeUsuario, Guid? conversacionId = null)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        // 1. Obtener o crear conversación
        HistorialChatResponse? respuestaHistorial = null;

        if (conversacionId != null && conversacionId != Guid.Empty)
        {
            respuestaHistorial = await _mediator.Send(new ObtenerHistorialChatQuery(conversacionId.Value));
            if (!respuestaHistorial.Existe)
            {
                conversacionId = null; // Marcar para creación si no existe
            }
        }

        if (conversacionId == null || conversacionId == Guid.Empty)
        {
            var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;
            conversacionId = await _mediator.Send(new CrearConversacionCommand(usuarioId));
            respuestaHistorial = await _mediator.Send(new ObtenerHistorialChatQuery(conversacionId.Value));
        }
        else if (respuestaHistorial == null)
        {
            respuestaHistorial = await _mediator.Send(new ObtenerHistorialChatQuery(conversacionId.Value));
        }

        // 2. Recuperar historial resumido (Regla 12 - Ventana Deslizante)
        // respuestaHistorial ya está cargado arriba.
        
        // Obtener los últimos 10 mensajes para el contexto inmediato (Aumentado para mejor fluidez)
        var mensajesRecientes = respuestaHistorial.Mensajes.OrderByDescending(m => m.Fecha).Take(10).OrderBy(m => m.Fecha).ToList();

        // 2.1 Poda Agresiva (Sprint 81 - Paso 2)
        // Si el historial crece demasiado, generamos un nuevo resumen y "vaciamos" la ventana activa
        if (respuestaHistorial.Mensajes.Count() > 15)
        {
            var nuevoResumen = await GenerarResumenAsync(conversacionId.Value);
            // Al generar un nuevo resumen, el siguiente ciclo usará este resumen y solo los mensajes nuevos.
            respuestaHistorial = await _mediator.Send(new ObtenerHistorialChatQuery(conversacionId.Value));
            mensajesRecientes = respuestaHistorial.Mensajes.OrderByDescending(m => m.Fecha).Take(5).OrderBy(m => m.Fecha).ToList();
        }

        // 3. Generar Snapshot de Estado (Regla 5) - Centralizado Sprint 82
        var snapshot = await _mediator.Send(new ObtenerDashboardSnapshotQuery());
        var sbSnapshot = new StringBuilder();
        sbSnapshot.AppendLine("SNAPSHOT DE ESTADO ACTUAL (TIEMPO REAL):");
        
        sbSnapshot.AppendLine("1. PRODUCCIÓN:");
        sbSnapshot.AppendLine($"   - Total Pollos Vivos: {snapshot.TotalPollosVivos}");
        sbSnapshot.AppendLine($"   - Mortalidad Mes Actual: {snapshot.MortalidadMesActual} bajas");
        foreach (var lote in snapshot.LotesActivos)
        {
            sbSnapshot.AppendLine($"   - Lote en {lote.GalponNombre}: {lote.CantidadActual} pollitos, {lote.EdadSemanas} semanas.");
        }

        sbSnapshot.AppendLine("2. INVENTARIO:");
        sbSnapshot.AppendLine($"   - Alimento: {snapshot.StockAlimentoKg} kg ({snapshot.DiasAlimentoRestantes} días restantes)");
        if (snapshot.RequiereAlertaAlimento) sbSnapshot.AppendLine("   - ⚠️ ALERTA: Stock de alimento crítico.");
        foreach (var alerta in snapshot.AlertasStockMinimo)
        {
            sbSnapshot.AppendLine($"   - ⚠️ STOCK BAJO: {alerta.ProductoNombre} ({alerta.StockActual} {alerta.Unidad})");
        }

        sbSnapshot.AppendLine("3. FINANZAS:");
        sbSnapshot.AppendLine($"   - Por Cobrar: S/ {snapshot.SaldoPorCobrarTotal}");
        sbSnapshot.AppendLine($"   - Por Pagar: S/ {snapshot.SaldoPorPagarTotal}");
        sbSnapshot.AppendLine($"   - Inversión en Lotes: S/ {snapshot.InversionTotalEnCurso}");

        sbSnapshot.AppendLine("4. SEGURIDAD Y AUDITORÍA (24h):");
        if (snapshot.AlertasSeguridad24h.Any())
        {
            foreach (var alerta in snapshot.AlertasSeguridad24h.Take(5))
            {
                sbSnapshot.AppendLine($"   - ⚠️ {alerta.UsuarioNombre} -> {alerta.Accion} en {alerta.EntidadNombre} ({alerta.Fecha:HH:mm}).");
            }
        }
        else
        {
            sbSnapshot.AppendLine("   - Sin eventos críticos de seguridad.");
        }

        sbSnapshot.AppendLine("5. SANIDAD:");
        sbSnapshot.AppendLine($"   - Tareas pendientes para hoy: {snapshot.TareasSanitariasHoy}");

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("Eres el Operador Maestro y Consultor Estratégico de GalponERP, un sistema de élite para la gestión avícola industrial. " +
                                     "Tu misión es el control total y la optimización proactiva de la granja. " +
                                     $"Fecha y hora actual: {DateTime.Now:f}. " +
                                     $"\n{sbSnapshot}\n" +
                                     "TUS CAPACIDADES INTEGRALES:" +
                                     "1. PRODUCCIÓN Y SANIDAD: Gestionas lotes, mortalidad, calendarios de vacunación y parámetros de bienestar (agua/temp)." +
                                     "2. ABASTECIMIENTO: Controlas inventario, creas órdenes de compra, recibes mercadería y gestionas deudas (Cuentas por Pagar)." +
                                     "3. ADMINISTRACIÓN Y SEGURIDAD: Gestionas usuarios, roles y auditas cada acción del sistema (Verdad Auditable)." +
                                     "4. ANÁLISIS IA: Correlacionas datos (ej. Alimento vs Mortalidad) y sugieres mejoras en el 'Modo Consultor'." +
                                     "\nREGLAS DE ORO:" +
                                     "1. BÚSQUEDA DIFUSA: Resuelves nombres de galpones, productos y usuarios aunque tengan errores tipográficos." +
                                     "2. CERO GUIDS: Nunca menciones IDs técnicos al usuario, usa siempre nombres legibles." +
                                     "3. PROACTIVIDAD: Si detectas anomalías (vencimientos de deudas, mortalidad alta), advierte al usuario de inmediato." +
                                     "4. CONFIRMACIÓN: Para acciones críticas (abrir/cerrar lotes, pagos, roles), pides confirmación antes de ejecutar." +
                                     "5. TONO: Profesional, ejecutivo y resolutivo.");

        // Inyectar Resumen si existe
        if (!string.IsNullOrEmpty(respuestaHistorial.Resumen))
        {
            chatHistory.AddSystemMessage($"RESUMEN DE LA CONVERSACIÓN PREVIA: {respuestaHistorial.Resumen}");
        }

        // Inyectar mensajes recientes
        foreach (var msg in mensajesRecientes)
        {
            if (msg.Rol.Equals("user", StringComparison.OrdinalIgnoreCase))
                chatHistory.AddUserMessage(msg.Contenido);
            else if (msg.Rol.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                chatHistory.AddAssistantMessage(msg.Contenido);
        }

        // 4. Agregar nuevo mensaje del usuario
        chatHistory.AddUserMessage(mensajeUsuario);
        
        // Guardar mensaje en persistencia
        await _mediator.Send(new GuardarMensajeCommand(conversacionId.Value, "user", mensajeUsuario));

        // Configuramos la invocación automática de funciones
        var executionSettings = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

        try
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, _kernel);
            var respuesta = result.Content ?? "No se generó respuesta del modelo.";

            // 5. Interceptar Ejecución de Pendientes
            if (respuesta.Contains("[EJECUTAR_PENDIENTE:"))
            {
                respuesta = await ProcesarEjecucionPendiente(respuesta, conversacionId.Value, chatHistory);
            }

            // 6. Guardar respuesta del asistente
            await _mediator.Send(new GuardarMensajeCommand(conversacionId.Value, "assistant", respuesta));

            // 6.1 Actualizar título si es el primer mensaje o el título es genérico (Sprint 84)
            if (respuestaHistorial.Mensajes.Count() <= 2)
            {
                await GenerarYActualizarTituloAsync(conversacionId.Value, mensajeUsuario);
            }

            // 7. Gestión de Memoria / Resumen (Sprint 72)
            // Si el total de mensajes traídos es >= 10, disparamos resumen
            if (respuestaHistorial.Mensajes.Count() >= 10)
            {
                await GenerarYGuardarResumenAsync(conversacionId.Value, respuestaHistorial.Resumen, respuestaHistorial.Mensajes);
            }

            return new AgenteResponse(respuesta, conversacionId.Value);
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error al procesar el mensaje: {ex.Message}";
            await _mediator.Send(new GuardarMensajeCommand(conversacionId.Value, "assistant", errorMsg));
            return new AgenteResponse(errorMsg, conversacionId.Value);
        }
    }

    public async Task<string> GenerarMensajeProactivoAsync(string contextoAnomalia)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("Eres el Operador Maestro de GalponERP. " +
                                     "Tu tarea es generar una ALERTA PROACTIVA para el administrador basada en una anomalía detectada por el sistema. " +
                                     "Sé directo, profesional y sugiere una acción correctiva si es posible. " +
                                     "No uses saludos genéricos largos, ve al grano pero con tono servicial. " +
                                     "Habla en español.");

        chatHistory.AddUserMessage($"Genera un mensaje de alerta para el siguiente problema: {contextoAnomalia}");

        try
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, kernel: _kernel);
            return result.Content ?? "Alerta detectada, pero no se pudo generar el mensaje.";
        }
        catch (Exception ex)
        {
            return $"Alerta automática: {contextoAnomalia}. (Error IA: {ex.Message})";
        }
    }

    public async Task<string> GenerarResumenAsync(Guid conversacionId)
    {
        var respuestaHistorial = await _mediator.Send(new ObtenerHistorialChatQuery(conversacionId));
        if (!respuestaHistorial.Mensajes.Any()) return "Conversación vacía.";

        var sb = new StringBuilder();
        foreach (var m in respuestaHistorial.Mensajes.OrderBy(m => m.Fecha))
        {
            sb.AppendLine($"{m.Rol}: {m.Contenido}");
        }

        var promptResumen = $"Resume los puntos clave de esta conversación avícola, manteniendo nombres de galpones, productos y cantidades. Sé muy conciso.\n\n{sb}";
        
        try
        {
            var result = await _kernel.InvokePromptAsync(promptResumen);
            var resumen = result.ToString();
            await _mediator.Send(new ActualizarResumenCommand(conversacionId, resumen, 0));
            return resumen;
        }
        catch
        {
            return "No se pudo generar el resumen técnico.";
        }
    }

    private async Task GenerarYGuardarResumenAsync(Guid conversacionId, string? resumenAnterior, IEnumerable<MensajeChatResponse> mensajes)
    {
        try
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(resumenAnterior))
            {
                sb.AppendLine($"Resumen anterior: {resumenAnterior}");
            }
            sb.AppendLine("Nuevos mensajes a resumir:");
            foreach (var m in mensajes)
            {
                sb.AppendLine($"{m.Rol}: {m.Contenido}");
            }

            var promptResumen = $"Resume de forma muy concisa los puntos clave de esta conversación avícola, manteniendo nombres de galpones, productos y cantidades mencionadas. No uses más de 100 palabras.\n\n{sb}";
            
            var result = await _kernel.InvokePromptAsync(promptResumen);
            var nuevoResumen = result.ToString();

            // Guardar el resumen en la BD
            await _mediator.Send(new ActualizarResumenCommand(conversacionId, nuevoResumen, 0));
        }
        catch
        {
            // Fallback silencioso
        }
    }

    private async Task<string> ProcesarEjecucionPendiente(string respuestaOriginal, Guid conversacionId, ChatHistory chatHistory)
    {
        try
        {
            var startIndex = respuestaOriginal.IndexOf("[EJECUTAR_PENDIENTE:") + "[EJECUTAR_PENDIENTE:".Length;
            var endIndex = respuestaOriginal.IndexOf("]", startIndex);
            var intencionIdStr = respuestaOriginal.Substring(startIndex, endIndex - startIndex);
            
            if (Guid.TryParse(intencionIdStr, out Guid intencionId))
            {
                var intencion = await _mediator.Send(new GalponERP.Application.Agentes.Confirmacion.Queries.ObtenerIntencionPendienteQuery(conversacionId));
                
                if (intencion != null && intencion.Id == intencionId)
                {
                    var plugin = _kernel.Plugins[intencion.PluginNombre];
                    var funcion = plugin[intencion.FuncionNombre];
                    
                    var dictParametros = JsonSerializer.Deserialize<Dictionary<string, object>>(intencion.ParametrosJson);
                    var kernelArguments = new KernelArguments();
                    if (dictParametros != null)
                    {
                        foreach (var kvp in dictParametros)
                        {
                            kernelArguments[kvp.Key] = kvp.Value?.ToString();
                        }
                    }
                    
                    kernelArguments["confirmar"] = "true";
                    
                    var result = await _kernel.InvokeAsync(funcion, kernelArguments);
                    var mensajeResultado = result.ToString() ?? "Acción ejecutada, pero no se recibió respuesta.";
                    
                    await _mediator.Send(new GalponERP.Application.Agentes.Confirmacion.Commands.MarcarIntencionComoProcesadaCommand(intencion.Id));
                    
                    var respuestaLimpia = respuestaOriginal.Replace($"[EJECUTAR_PENDIENTE:{intencionId}]", "").Trim();
                    return $"{respuestaLimpia}\n\nResultado de la operación: {mensajeResultado}";
                }
            }
            return respuestaOriginal;
        }
        catch (Exception ex)
        {
            return $"{respuestaOriginal}\n\nError al ejecutar la acción pendiente: {ex.Message}";
        }
    }

    private async Task GenerarYActualizarTituloAsync(Guid conversacionId, string mensajeUsuario)
    {
        try
        {
            var promptTitulo = $"Genera un título muy corto (máximo 5 palabras) y descriptivo para una conversación que empieza con este mensaje: \"{mensajeUsuario}\". No uses comillas ni puntos al final.";
            var result = await _kernel.InvokePromptAsync(promptTitulo);
            var nuevoTitulo = result.ToString().Trim();

            if (!string.IsNullOrEmpty(nuevoTitulo))
            {
                await _mediator.Send(new GalponERP.Application.Agentes.Chat.Commands.ActualizarTitulo.ActualizarTituloCommand(conversacionId, nuevoTitulo));
            }
        }
        catch
        {
            // Fallback silencioso
        }
    }
}
