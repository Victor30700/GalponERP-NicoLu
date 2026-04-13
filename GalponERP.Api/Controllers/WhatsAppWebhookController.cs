using System.Text.Json;
using GalponERP.Application.Agentes;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IAgenteOrquestadorService _agenteOrquestador;
    private readonly IWhatsAppService _whatsAppService;
    private readonly IVoiceService _voiceService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConversacionRepository _conversacionRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WhatsAppWebhookController> _logger;

    public WhatsAppWebhookController(
        IAgenteOrquestadorService agenteOrquestador,
        IWhatsAppService whatsAppService,
        IVoiceService voiceService,
        IUsuarioRepository usuarioRepository,
        IConversacionRepository conversacionRepository,
        ICurrentUserContext currentUserContext,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<WhatsAppWebhookController> logger)
    {
        _agenteOrquestador = agenteOrquestador;
        _whatsAppService = whatsAppService;
        _voiceService = voiceService;
        _usuarioRepository = usuarioRepository;
        _conversacionRepository = conversacionRepository;
        _currentUserContext = currentUserContext;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Verificación del Webhook por parte de Meta (GET)
    /// </summary>
    [HttpGet("webhook")]
    public IActionResult VerifyWebhook(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.verify_token")] string verifyToken,
        [FromQuery(Name = "hub.challenge")] string challenge)
    {
        var secretToken = _configuration["WhatsApp:VerifyToken"];

        if (mode == "subscribe" && verifyToken == secretToken)
        {
            _logger.LogInformation("Webhook de WhatsApp verificado con éxito.");
            return Ok(challenge);
        }

        return Forbid();
    }

    /// <summary>
    /// Recepción de mensajes de WhatsApp (POST)
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> ReceiveMessage([FromBody] JsonElement body)
    {
        try
        {
            _logger.LogInformation("Mensaje recibido de WhatsApp: {Body}", body.GetRawText());

            // 1. Extraer datos básicos del mensaje
            if (!TryExtractMessageData(body, out var telefono, out var mensaje, out var mediaId))
            {
                return Ok();
            }

            // 2. Buscar usuario por WhatsApp vinculado
            var usuario = await _usuarioRepository.ObtenerPorWhatsAppAsync(telefono);
            
            if (usuario == null)
            {
                // Intento de vinculación: ¿El mensaje es un código de 6 dígitos?
                if (!string.IsNullOrEmpty(mensaje) && mensaje.Trim().Length == 6 && int.TryParse(mensaje.Trim(), out _))
                {
                    var codigo = mensaje.Trim();
                    var usuarioPendiente = await _usuarioRepository.ObtenerPorCodigoVinculacionAsync(codigo);
                    
                    if (usuarioPendiente != null && usuarioPendiente.ValidarCodigoVinculacion(codigo))
                    {
                        usuarioPendiente.VincularWhatsApp(telefono);
                        _usuarioRepository.Actualizar(usuarioPendiente);
                        await _unitOfWork.SaveChangesAsync();

                        await _whatsAppService.EnviarMensajeTextoAsync(telefono, 
                            $"¡Éxito! Tu número ha sido vinculado a la cuenta de {usuarioPendiente.Nombre}. Ahora puedes enviarme comandos por aquí.");
                        return Ok();
                    }
                }

                await _whatsAppService.EnviarMensajeTextoAsync(telefono, 
                    "Hola de GalponERP. Este número no está vinculado a ninguna cuenta. " +
                    "Para vincularlo, entra a tu perfil en la web, genera un 'Código de WhatsApp' y envíamelo por aquí.");
                return Ok();
            }

            // 3. Establecer contexto de usuario para auditoría
            if (_currentUserContext is CurrentUserContext context)
            {
                context.SetUser(usuario.Id, usuario.FirebaseUid);
            }

            // 4. Procesar audio si es necesario
            if (!string.IsNullOrEmpty(mediaId))
            {
                var audioBytes = await _whatsAppService.DescargarMediaAsync(mediaId);
                if (audioBytes.Length > 0)
                {
                    using var ms = new MemoryStream(audioBytes);
                    mensaje = await _voiceService.TranscribirAudioAsync(ms, "voice.ogg");
                    _logger.LogInformation("Transcripción de voz: {Texto}", mensaje);
                }
            }

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return Ok();
            }

            // 5. Obtener conversación activa
            var conversaciones = await _conversacionRepository.ObtenerPorUsuarioAsync(usuario.Id);
            var conversacionActiva = conversaciones.FirstOrDefault(c => c.Estado == "Activa");

            // 6. Procesar con el Agente Orquestador
            var respuesta = await _agenteOrquestador.ProcesarMensajeAsync(mensaje, conversacionActiva?.Id);

            // 7. Enviar respuesta por WhatsApp
            await _whatsAppService.EnviarMensajeTextoAsync(telefono, respuesta.Respuesta);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar webhook de WhatsApp");
            return Ok();
        }
    }

    private bool TryExtractMessageData(JsonElement body, out string telefono, out string mensaje, out string mediaId)
    {
        telefono = string.Empty;
        mensaje = string.Empty;
        mediaId = string.Empty;

        try
        {
            var entry = body.GetProperty("entry")[0];
            var changes = entry.GetProperty("changes")[0];
            var value = changes.GetProperty("value");
            
            if (value.TryGetProperty("messages", out var messages))
            {
                var message = messages[0];
                telefono = message.GetProperty("from").GetString() ?? string.Empty;
                var type = message.GetProperty("type").GetString();

                if (type == "text")
                {
                    mensaje = message.GetProperty("text").GetProperty("body").GetString() ?? string.Empty;
                    return !string.IsNullOrEmpty(telefono) && !string.IsNullOrEmpty(mensaje);
                }
                else if (type == "audio")
                {
                    mediaId = message.GetProperty("audio").GetProperty("id").GetString() ?? string.Empty;
                    return !string.IsNullOrEmpty(telefono) && !string.IsNullOrEmpty(mediaId);
                }
            }
        }
        catch
        {
            // Payload no compatible o incompleto
        }

        return false;
    }
}
