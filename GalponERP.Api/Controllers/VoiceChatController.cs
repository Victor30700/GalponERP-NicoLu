using GalponERP.Application.Agentes;
using GalponERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/voice")]
public class VoiceChatController : ControllerBase
{
    private readonly IAgenteOrquestadorService _agenteOrquestador;
    private readonly IVoiceService _voiceService;
    private readonly ILogger<VoiceChatController> _logger;

    public VoiceChatController(
        IAgenteOrquestadorService agenteOrquestador,
        IVoiceService voiceService,
        ILogger<VoiceChatController> logger)
    {
        _agenteOrquestador = agenteOrquestador;
        _voiceService = voiceService;
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

            // 2. Procesar con el Agente
            var response = await _agenteOrquestador.ProcesarMensajeAsync(transcript, request.ConversacionId);

            // 3. Sintetizar respuesta a audio (TTS)
            var audioBytes = await _voiceService.SintetizarVozAsync(response.Respuesta);

            return Ok(new VoiceChatResponse
            {
                Transcripcion = transcript,
                RespuestaTexto = response.Respuesta,
                RespuestaAudioBase64 = Convert.ToBase64String(audioBytes),
                ConversacionId = response.ConversacionId
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
