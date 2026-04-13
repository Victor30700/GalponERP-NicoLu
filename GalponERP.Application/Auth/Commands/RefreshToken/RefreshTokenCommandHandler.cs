using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace GalponERP.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Firebase:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Firebase ApiKey is not configured in appsettings.json");
        }

        var url = $"https://securetoken.googleapis.com/v1/token?key={apiKey}";
        
        var payload = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", request.RefreshToken }
        };

        var content = new FormUrlEncodedContent(payload);
        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Refresh token failed: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<FirebaseRefreshTokenResponse>(cancellationToken: cancellationToken);

        if (result == null)
        {
            throw new Exception("Refresh token failed: Empty response from Firebase");
        }

        // El endpoint de refresh token de Firebase no devuelve el email directamente.
        // Lo extraemos del id_token (JWT) si es posible para mantener consistencia con LoginResponse.
        string email = string.Empty;
        try
        {
            var parts = result.IdToken.Split('.');
            if (parts.Length > 1)
            {
                var jwtPayload = parts[1];
                // Base64Url decode logic
                jwtPayload = jwtPayload.Replace('-', '+').Replace('_', '/');
                switch (jwtPayload.Length % 4)
                {
                    case 2: jwtPayload += "=="; break;
                    case 3: jwtPayload += "="; break;
                }
                var bytes = Convert.FromBase64String(jwtPayload);
                var decodedPayload = System.Text.Encoding.UTF8.GetString(bytes);
                using var doc = System.Text.Json.JsonDocument.Parse(decodedPayload);
                if (doc.RootElement.TryGetProperty("email", out var emailProp))
                {
                    email = emailProp.GetString() ?? string.Empty;
                }
            }
        }
        catch 
        { 
            // Fallback a string vacío si la decodificación falla
        }

        return new RefreshTokenResponse(
            result.IdToken,
            result.RefreshToken,
            email,
            int.Parse(result.ExpiresIn)
        );
    }
}

public class FirebaseRefreshTokenResponse
{
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; } = string.Empty;
}
