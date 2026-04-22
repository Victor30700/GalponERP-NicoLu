using System.Text.Json;
using System.Text;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IVoiceService _voiceService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WhatsAppWebhookController> _logger;

    public WhatsAppWebhookController(
        IWhatsAppService whatsAppService,
        IVoiceService voiceService,
        IUsuarioRepository usuarioRepository,
        ICurrentUserContext currentUserContext,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<WhatsAppWebhookController> logger)
    {
        _whatsAppService = whatsAppService;
        _voiceService = voiceService;
        _usuarioRepository = usuarioRepository;
        _currentUserContext = currentUserContext;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

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

    [HttpPost("webhook")]
    public async Task<IActionResult> ReceiveMessage([FromBody] JsonElement body)
    {
        try
        {
            _logger.LogInformation("Mensaje recibido de WhatsApp: {Body}", body.GetRawText());

            if (!TryExtractMessageData(body, out var telefono, out var mensaje, out var mediaId))
            {
                return Ok();
            }

            var usuario = await _usuarioRepository.ObtenerPorWhatsAppAsync(telefono);
            
            if (usuario == null)
            {
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

            // Establecer contexto de usuario para auditoría (aunque el orquestador ahora es n8n)
            if (_currentUserContext is CurrentUserContext context)
            {
                context.SetUser(usuario.Id, usuario.FirebaseUid);
            }

            if (!string.IsNullOrEmpty(mediaId))
            {
                var audioBytes = await _whatsAppService.DescargarMediaAsync(mediaId);
                if (audioBytes.Length > 0)
                {
                    using var ms = new MemoryStream(audioBytes);
                    mensaje = await _voiceService.TranscribirAudioAsync(ms, "voice.ogg");
                }
            }

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return Ok();
            }

            // Reenviar a n8n
            var n8nWebhookUrl = _configuration["Integrations:N8nWhatsAppWebhookUrl"];
            if (!string.IsNullOrEmpty(n8nWebhookUrl))
            {
                var payload = new
                {
                    UsuarioId = usuario.Id,
                    Nombre = usuario.Nombre,
                    Telefono = telefono,
                    Mensaje = mensaje,
                    FullBody = body
                };

                using var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                await client.PostAsync(n8nWebhookUrl, content);
            }

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
