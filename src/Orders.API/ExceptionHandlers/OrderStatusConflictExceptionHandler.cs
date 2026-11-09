using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class OrderStatusConflictExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OrderStatusConflictException error)
        {
            return false;
        }
        context.Items["errorCode"] = OrderStatusConflictException.ErrorCode;
        await Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            detail: error.Message,
            instance: context.Request.Path.Value,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = OrderStatusConflictException.ErrorCode,
                ["errorMessage"] = error.Message,
                ["correlationId"] = context.TraceIdentifier
            }).ExecuteAsync(context);

        return true;
    }
}