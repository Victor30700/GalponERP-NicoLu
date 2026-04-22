using System.Diagnostics;
using GalponERP.Application.Exceptions;
using GalponERP.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GalponERP.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var request = httpContext.Request;
        var maskedHeaders = request.Headers
            .Where(h => h.Key.ToLower() != "authorization") // También ocultamos Authorization por seguridad
            .Select(h => $"{h.Key}: {(h.Key.ToLower() == "x-api-key" ? "***MASKED***" : h.Value.ToString())}");

        _logger.LogError(
            exception,
            "Exception occurred on {Method} {Path}. Headers: {Headers}. Message: {Message}",
            request.Method,
            request.Path,
            string.Join(", ", maskedHeaders),
            exception.Message);

        (int statusCode, string title, string detail, object? errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status422UnprocessableEntity,
                "Validation Error",
                validationException.Message,
                (object?)validationException.Errors),
            DomainException domainException => (
                StatusCodes.Status400BadRequest,
                "Domain Error",
                domainException.Message,
                null),
            KeyNotFoundException keyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                keyNotFoundException.Message,
                null),
            GalponERP.Application.Exceptions.ConcurrencyException concurrencyException => (
                StatusCodes.Status409Conflict,
                "Concurrency Error",
                concurrencyException.Message,
                null),
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Concurrency Error",
                "El registro fue modificado o eliminado por otro usuario. Por favor, recargue los datos e intente nuevamente.",
                null),
            DbUpdateException dbUpdateException => (
                StatusCodes.Status400BadRequest,
                "Database Error",
                "Ocurrió un error al persistir los cambios en la base de datos. Verifique que todos los registros relacionados existan y que no haya duplicados.",
                null),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected error occurred",
                null)
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
