using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class InsufficientStockExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not InsufficientStockException error)
        {
            return false;
        }

        await Results.Problem(
            statusCode: StatusCodes.Status422UnprocessableEntity,
            detail: error.Message,
            instance: context.Request.Path.Value,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = InsufficientStockException.ErrorCode,
                ["errorMessage"] = error.Message,
                ["correlationId"] = context.TraceIdentifier
            }).ExecuteAsync(context);

        return true;
    }
}