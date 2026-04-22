using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using GalponERP.Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace GalponERP.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string APIKEYNAME = "x-api-key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            await _next(context);
            return;
        }

        var apiKey = configuration.GetValue<string>("Auth:ApiKey");

        if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid API Key.");
            return;
        }

        // If API Key is valid, we set the Service Account user
        var serviceAccountUid = "service-account-n8n";
        var usuarioRepository = context.RequestServices.GetRequiredService<IUsuarioRepository>();
        var currentUserContext = context.RequestServices.GetRequiredService<ICurrentUserContext>() as CurrentUserContext;

        var usuario = await usuarioRepository.ObtenerPorFirebaseUidAsync(serviceAccountUid);

        if (usuario != null && usuario.Active == 1)
        {
            currentUserContext?.SetUser(usuario.Id, serviceAccountUid, usuario.Nombre);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("user_id", serviceAccountUid)
            };

            var identity = new ClaimsIdentity(claims, "ApiKey");
            context.User = new ClaimsPrincipal(identity);
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Service Account not found or inactive.");
            return;
        }

        await _next(context);
    }
}
