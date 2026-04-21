using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace GalponERP.Api.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private const string IdempotencyHeader = "X-Idempotency-Key";

    public IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Solo aplicar a POST y PUT
        if (context.Request.Method != HttpMethods.Post && context.Request.Method != HttpMethods.Put)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(IdempotencyHeader, out var idempotencyKey))
        {
            await _next(context);
            return;
        }

        string cacheKey = $"idempotency_{idempotencyKey}";

        if (_cache.TryGetValue(cacheKey, out byte[]? responseBody))
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            await context.Response.Body.WriteAsync(responseBody!);
            return;
        }

        // Interceptar la respuesta
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        // Si la respuesta es exitosa (2xx), cachearla
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBodyArray = responseBodyStream.ToArray();
            
            // Cachear por 1 hora
            _cache.Set(cacheKey, responseBodyArray, TimeSpan.FromHours(1));

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
        else
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}
