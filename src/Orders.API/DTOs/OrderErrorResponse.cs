using Microsoft.AspNetCore.Mvc;

namespace Orders.API.DTOs;

/// <summary>Formato de error documentado para Orders.API.</summary>
public class OrderErrorResponse : ProblemDetails
{
    /// <summary>Código de error del catálogo de Orders, por ejemplo ORD-005.</summary>
    public required string ErrorCode { get; init; }

    /// <summary>Mensaje que explica el error.</summary>
    public required string ErrorMessage { get; init; }

    /// <summary>Identificador que permite encontrar la solicitud en los logs.</summary>
    public required string CorrelationId { get; init; }
}