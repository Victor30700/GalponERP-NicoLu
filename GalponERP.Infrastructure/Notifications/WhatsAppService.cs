using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GalponERP.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GalponERP.Infrastructure.Notifications;

public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private readonly string _phoneNumberId;
    private readonly string _apiVersion;
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(IConfiguration configuration, HttpClient httpClient, ILogger<WhatsAppService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        var config = configuration.GetSection("WhatsApp");
        _accessToken = config["AccessToken"] ?? string.Empty;
        _phoneNumberId = config["PhoneNumberId"] ?? string.Empty;
        _apiVersion = config["ApiVersion"] ?? "v18.0";
    }

    public async Task<bool> EnviarMensajeTextoAsync(string telefono, string mensaje)
    {
        var url = $"https://graph.facebook.com/{_apiVersion}/{_phoneNumberId}/messages";

        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = telefono,
            type = "text",
            text = new { body = mensaje }
        };

        return await EnviarRequestAsync(url, payload);
    }

    public async Task<bool> EnviarNotificacionPlantillaAsync(string telefono, string nombrePlantilla, string lenguaje = "es_ES")
    {
        var url = $"https://graph.facebook.com/{_apiVersion}/{_phoneNumberId}/messages";

        var payload = new
        {
            messaging_product = "whatsapp",
            to = telefono,
            type = "template",
            template = new
            {
                name = nombrePlantilla,
                language = new { code = lenguaje }
            }
        };

        return await EnviarRequestAsync(url, payload);
    }

    public async Task<byte[]> DescargarMediaAsync(string mediaId)
    {
        try
        {
            // 1. Obtener la URL del media
            var url = $"https://graph.facebook.com/{_apiVersion}/{mediaId}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return Array.Empty<byte>();

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            var downloadUrl = jsonDoc.RootElement.GetProperty("url").GetString();

            if (string.IsNullOrEmpty(downloadUrl)) return Array.Empty<byte>();

            // 2. Descargar el archivo
            using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
            downloadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            downloadRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0"); // A veces es necesario para la API de Meta

            var downloadResponse = await _httpClient.SendAsync(downloadRequest);
            if (!downloadResponse.IsSuccessStatusCode) return Array.Empty<byte>();

            return await downloadResponse.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar media de WhatsApp");
            return Array.Empty<byte>();
        }
    }

    private async Task<bool> EnviarRequestAsync(string url, object payload)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            
            var jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al enviar mensaje a WhatsApp: {StatusCode} - {Content}", response.StatusCode, content);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al enviar mensaje a WhatsApp");
            return false;
        }
    }
}
