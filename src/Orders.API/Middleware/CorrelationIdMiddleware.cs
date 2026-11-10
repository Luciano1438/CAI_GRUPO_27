using System.Diagnostics;
using Serilog.Context;

namespace Orders.API.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;
private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
    RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
       _next = next;
       _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = context.Request.Headers[HeaderName].ToString();

        if (!EsValido(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.TraceIdentifier = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
using (LogContext.PushProperty("Endpoint", context.Request.Path.Value))
{
    var reloj = Stopwatch.StartNew();

    _logger.LogInformation("Inicio de {Metodo}", context.Request.Method);

    await _next(context);

    int status = context.Response.StatusCode;
    LogLevel nivel = LogLevel.Information;

    if (status >= 500)
    {
        nivel = LogLevel.Error;
    }
    else if (status >= 400)
    {
        nivel = LogLevel.Warning;
    }

    context.Items.TryGetValue("errorCode", out object? codigo);

    using (LogContext.PushProperty("errorCode", codigo))
    {
        _logger.Log(nivel,
            "Fin de {Metodo}: HTTP {StatusCode} en {DuracionMs} ms",
            context.Request.Method, status, reloj.Elapsed.TotalMilliseconds);
    }
}
    }

    private static bool EsValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Length > 100)
        {
            return false;
        }

        foreach (char caracter in valor)
        {
            if (caracter < 33 || caracter > 126)
            {
                return false;
            }
        }

        return true;
    }
}