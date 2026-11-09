namespace Orders.API.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
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

        await _next(context);
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