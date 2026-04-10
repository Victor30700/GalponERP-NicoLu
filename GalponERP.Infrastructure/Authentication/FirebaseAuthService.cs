using GalponERP.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GalponERP.Infrastructure.Authentication;

public class FirebaseAuthService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FirebaseAuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string?> GetUserIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // En Firebase, a veces se usa un claim personalizado 'user_id' si 'NameIdentifier' no está disponible
        if (string.IsNullOrEmpty(userId))
        {
            userId = _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value;
        }

        return Task.FromResult(userId);
    }

    public Task<string?> GetUserEmailAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Task.FromResult(email);
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
