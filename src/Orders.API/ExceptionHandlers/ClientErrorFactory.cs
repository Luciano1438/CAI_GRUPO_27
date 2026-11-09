using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class ClientErrorFactory : IClientErrorFactory
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ClientErrorFactory(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public IActionResult GetClientError(
        ActionContext context, IClientErrorActionResult clientError)
    {
        var http = context.HttpContext;
        int status = clientError.StatusCode ?? StatusCodes.Status400BadRequest;
        var problema = _problemDetailsFactory.CreateProblemDetails(
            http, statusCode: status, instance: http.Request.Path.Value);

        if (status == StatusCodes.Status415UnsupportedMediaType)
        {
            const string mensaje =
                "El tipo de contenido no es compatible. Enviá application/json.";

            problema.Detail = mensaje;
            problema.Extensions["errorCode"] = OrderValidationException.ErrorCode;
            problema.Extensions["errorMessage"] = mensaje;
            problema.Extensions["correlationId"] = http.TraceIdentifier;
        }

        var respuesta = new ObjectResult(problema) { StatusCode = status };
        respuesta.ContentTypes.Add("application/problem+json");
        return respuesta;
    }
}