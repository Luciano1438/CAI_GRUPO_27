using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class OrderValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OrderValidationException error)
        {
            return false;
        }
        context.Items["errorCode"] = OrderValidationException.ErrorCode;
        var problema = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = error.Message,
            Instance = context.Request.Path.Value
        };

        problema.Extensions["errorCode"] = OrderValidationException.ErrorCode;
        problema.Extensions["errorMessage"] = error.Message;
        problema.Extensions["correlationId"] = context.TraceIdentifier;

        context.Response.StatusCode = 400;

        await context.Response.WriteAsJsonAsync(
           problema,
           options: (System.Text.Json.JsonSerializerOptions?)null,
           contentType: "application/problem+json",
           cancellationToken: cancellationToken);

        return true;
    }
}