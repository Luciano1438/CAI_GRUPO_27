using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Orders.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        const string codigo = "ORD-007";
        const string mensaje = "Error interno al procesar la orden.";
        
        context.Items["errorCode"] = codigo;
        _logger.LogError(
            exception,
            "Error inesperado en Orders. ErrorCode: {ErrorCode}, CorrelationId: {CorrelationId}",
            codigo,
            context.TraceIdentifier);

        var problema = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Title = "Internal Server Error",
            Status = 500,
            Detail = mensaje,
            Instance = context.Request.Path.Value
        };

        problema.Extensions["errorCode"] = codigo;
        problema.Extensions["errorMessage"] = mensaje;
        problema.Extensions["correlationId"] = context.TraceIdentifier;

        context.Response.StatusCode = 500;

        await context.Response.WriteAsJsonAsync(
            problema,
            options: (System.Text.Json.JsonSerializerOptions?)null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }
}