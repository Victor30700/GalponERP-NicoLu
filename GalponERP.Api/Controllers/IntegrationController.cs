using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace GalponERP.Api.Controllers;

[ApiController]
[Route("api/integration")]
public class IntegrationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IntegrationController> _logger;

    public IntegrationController(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration, 
        ILogger<IntegrationController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Recibe webhooks de Telegram y los reenvía a n8n para orquestación de IA.
    /// </summary>
    [HttpPost("telegram")]
    public async Task<IActionResult> TelegramWebhook([FromBody] JsonElement body)
    {
        try
        {
            // Validación de Token de Seguridad de Telegram (Blindaje contra ataques de timing)
            var expectedToken = _configuration["Telegram:WebhookToken"];
            if (!HttpContext.Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var receivedToken) ||
                string.IsNullOrEmpty(expectedToken))
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
                _logger.LogWarning("Intento de acceso no autorizado al gateway de Telegram desde IP: {IP}. Token ausente o configuración incompleta.", clientIp);
                return Unauthorized();
            }

            var receivedTokenStr = receivedToken.ToString();
            var expectedBytes = Encoding.UTF8.GetBytes(expectedToken);
            var receivedBytes = Encoding.UTF8.GetBytes(receivedTokenStr);

            if (expectedBytes.Length != receivedBytes.Length ||
                !CryptographicOperations.FixedTimeEquals(receivedBytes, expectedBytes))
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
                _logger.LogWarning("Intento de acceso no autorizado al gateway de Telegram desde IP: {IP}. Token inválido.", clientIp);
                return Unauthorized();
            }

            _logger.LogInformation("Telegram webhook received: {Body}", body.GetRawText());

            var n8nWebhookUrl = _configuration["Integrations:N8nTelegramWebhookUrl"];
            if (string.IsNullOrEmpty(n8nWebhookUrl))
            {
                _logger.LogWarning("n8n Telegram webhook URL is not configured.");
                return Ok(); // Respondemos OK a Telegram para evitar reintentos infinitos
            }

            using var client = _httpClientFactory.CreateClient();
            var content = new StringContent(body.GetRawText(), Encoding.UTF8, "application/json");
            
            // Opcional: Podríamos añadir headers de seguridad si n8n lo requiere
            var response = await client.PostAsync(n8nWebhookUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error forwarding to n8n: {StatusCode}", response.StatusCode);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in TelegramWebhook");
            return Ok();
        }
    }
}
