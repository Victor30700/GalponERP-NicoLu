using System.Net.Http.Json;
using System.Text.Json.Serialization;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace GalponERP.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginCommandHandler(HttpClient httpClient, IConfiguration configuration, IUsuarioRepository usuarioRepository)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Firebase:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Firebase ApiKey is not configured in appsettings.json");
        }

        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";
        
        var payload = new
        {
            email = request.Email,
            password = request.Password,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>(cancellationToken: cancellationToken);
            throw new Exception($"Login failed: {error?.Error?.Message ?? "Unknown error"}");
        }

        var result = await response.Content.ReadFromJsonAsync<FirebaseLoginResponse>(cancellationToken: cancellationToken);

        if (result == null)
        {
            throw new Exception("Login failed: Empty response from Firebase");
        }

        // 4. Verificar si el usuario está activo en la base de datos local
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(result.Email);
        if (usuario != null && (usuario.Active == 0 || !usuario.IsActive))
        {
            throw new Exception("El usuario no está activo en el sistema. Contacte al administrador.");
        }

        return new LoginResponse(
            result.IdToken,
            result.RefreshToken,
            result.Email,
            int.Parse(result.ExpiresIn)
        );
    }
}

public class FirebaseLoginResponse
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("expiresIn")]
    public string ExpiresIn { get; set; } = string.Empty;

    [JsonPropertyName("localId")]
    public string LocalId { get; set; } = string.Empty;

    [JsonPropertyName("registered")]
    public bool Registered { get; set; }
}

public class FirebaseErrorResponse
{
    [JsonPropertyName("error")]
    public FirebaseErrorDetail? Error { get; set; }
}

public class FirebaseErrorDetail
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
