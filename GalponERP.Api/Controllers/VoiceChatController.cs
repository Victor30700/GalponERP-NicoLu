using System.Text;
using System.Text.Json;
using GalponERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/voice")]
public class VoiceChatController : ControllerBase
{
    private readonly IVoiceService _voiceService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VoiceChatController> _logger;

    public VoiceChatController(
        IVoiceService voiceService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<VoiceChatController> logger)
    {
        _voiceService = voiceService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<VoiceChatResponse>> UploadVoice([FromForm] VoiceChatRequest request)
    {
        try
        {
            if (request.Audio == null || request.Audio.Length == 0)
                return BadRequest("No se proporcionó archivo de audio.");

            // 1. Transcribir audio a texto (STT)
            using var stream = request.Audio.OpenReadStream();
            var transcript = await _voiceService.TranscribirAudioAsync(stream, request.Audio.FileName);

            if (string.IsNullOrWhiteSpace(transcript))
                return BadRequest("No se pudo transcribir el audio.");

            // 2. Reenviar a n8n para obtener respuesta de IA
            var n8nWebhookUrl = _configuration["Integrations:N8nVoiceChatWebhookUrl"];
            if (string.IsNullOrEmpty(n8nWebhookUrl))
            {
                return StatusCode(500, "n8n voice chat webhook URL is not configured.");
            }

            var payload = new
            {
                Transcripcion = transcript,
                ConversacionId = request.ConversacionId
            };

            using var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(n8nWebhookUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error processing with n8n.");
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resultJson);
            var respuestaTexto = doc.RootElement.GetProperty("respuesta").GetString() ?? string.Empty;
            var nuevaConversacionId = request.ConversacionId ?? (doc.RootElement.TryGetProperty("conversacionId", out var idProp) ? idProp.GetGuid() : Guid.Empty);

            // 3. Sintetizar respuesta a audio (TTS)
            var audioBytes = await _voiceService.SintetizarVozAsync(respuestaTexto);

            return Ok(new VoiceChatResponse
            {
                Transcripcion = transcript,
                RespuestaTexto = respuestaTexto,
                RespuestaAudioBase64 = Convert.ToBase64String(audioBytes),
                ConversacionId = nuevaConversacionId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en chat de voz");
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class VoiceChatRequest
{
    public IFormFile Audio { get; set; } = null!;
    public Guid? ConversacionId { get; set; }
}

public class VoiceChatResponse
{
    public string Transcripcion { get; set; } = string.Empty;
    public string RespuestaTexto { get; set; } = string.Empty;
    public string RespuestaAudioBase64 { get; set; } = string.Empty;
    public Guid ConversacionId { get; set; }
}
