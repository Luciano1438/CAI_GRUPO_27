using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class OrderNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OrderNotFoundException error)
        {
            return false;
        }

        var problema = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "Not Found",
            Status = 404,
            Detail = error.Message,
            Instance = context.Request.Path.Value
        };

        problema.Extensions["errorCode"] = error.Codigo;
        problema.Extensions["errorMessage"] = error.Message;
        problema.Extensions["correlationId"] = context.TraceIdentifier;

        context.Response.StatusCode = 404;

        await context.Response.WriteAsJsonAsync(
            problema,
            options: (System.Text.Json.JsonSerializerOptions?)null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }
}