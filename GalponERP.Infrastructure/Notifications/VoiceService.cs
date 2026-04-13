using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GalponERP.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GalponERP.Infrastructure.Notifications;

public class VoiceService : IVoiceService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<VoiceService> _logger;

    public VoiceService(IConfiguration configuration, HttpClient httpClient, ILogger<VoiceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    }

    public async Task<string> TranscribirAudioAsync(Stream audioStream, string fileName)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(audioStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            
            content.Add(streamContent, "file", fileName);
            content.Add(new StringContent("whisper-1"), "model");
            content.Add(new StringContent("es"), "language");

            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error en OpenAI Whisper: {StatusCode} - {Content}", response.StatusCode, responseBody);
                return string.Empty;
            }

            using var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement.GetProperty("text").GetString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al transcribir audio");
            return string.Empty;
        }
    }

    public async Task<byte[]> SintetizarVozAsync(string texto)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/speech");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var payload = new
            {
                model = "tts-1",
                input = texto,
                voice = "alloy"
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error en OpenAI TTS: {StatusCode} - {Content}", response.StatusCode, error);
                return Array.Empty<byte>();
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al sintetizar voz");
            return Array.Empty<byte>();
        }
    }
}
