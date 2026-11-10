using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Orders.API.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Orders.API.Swagger;

public class CrearOrdenExamplesFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    if (context.MethodInfo.DeclaringType == typeof(OrdersController)
&& operation.Responses is not null
&& operation.Responses.TryGetValue("200", out var respuesta)
&& respuesta.Content is not null
&& respuesta.Content.TryGetValue("application/json", out var contenido))
    {
      var orden = JsonNode.Parse("""
        {
          "id": "11111111-1111-4111-8111-111111111111",
          "usuarioId": "22222222-2222-4222-8222-222222222222",
          "items": [
            {
              "productoId": "33333333-3333-4333-8333-333333333333",
              "cantidad": 2,
              "precioUnitario": 12.50
            }
          ],
          "total": 25.00,
          "estado": "Pendiente",
          "fechaCreacion": "2026-11-09T16:00:00Z"
        }
        """);

      contenido.Example = context.MethodInfo.Name switch
      {
        nameof(OrdersController.Listar) => new JsonArray(orden),
        nameof(OrdersController.Buscar) => orden,
        nameof(OrdersController.CambiarEstado) => JsonNode.Parse("""
            {
              "id": "11111111-1111-4111-8111-111111111111",
              "estado": "Confirmada",
              "fechaActualizacion": "2026-11-09T16:05:00Z"
            }
            """),
        _ => contenido.Example
      };
    }

        if (context.MethodInfo.DeclaringType == typeof(OrdersController))
    {
      const string id = "11111111-1111-4111-8111-111111111111";

      switch (context.MethodInfo.Name)
      {
        case nameof(OrdersController.Crear):
          AgregarEjemploError("400", "Bad Request", "ORD-002",
              "Indicá el ID del usuario.", "/api/orders");
          break;
        case nameof(OrdersController.Buscar):
          AgregarEjemploError("404", "Not Found", "ORD-001",
              "Orden no encontrada.", $"/api/orders/{id}");
          break;
        case nameof(OrdersController.CambiarEstado):
          AgregarEjemploError("409", "Conflict", "ORD-006",
              "No se puede cambiar de Confirmada a Pendiente.",
              $"/api/orders/{id}/status");
          break;
      }
    }

    void AgregarEjemploError(string codigoHttp, string titulo, string codigoError,
        string mensaje, string ruta)
    {
      if (operation.Responses?.TryGetValue(codigoHttp, out var error) == true
          && error.Content?.TryGetValue("application/problem+json", out var json) == true)
      {
        json.Example = new JsonObject
        {
          ["title"] = titulo,
          ["status"] = int.Parse(codigoHttp),
          ["detail"] = mensaje,
          ["instance"] = ruta,
          ["errorCode"] = codigoError,
          ["errorMessage"] = mensaje,
          ["correlationId"] = $"demo-orders-{codigoHttp}"
        };
      }
    }


    if (context.MethodInfo.DeclaringType != typeof(OrdersController)
        || context.MethodInfo.Name != nameof(OrdersController.Crear))
    {
      return;
    }

    operation.Responses!["201"].Content!["application/json"].Example =
        JsonNode.Parse("""
            {
              "id": "11111111-1111-4111-8111-111111111111",
              "usuarioId": "22222222-2222-4222-8222-222222222222",
              "items": [
                {
                  "productoId": "33333333-3333-4333-8333-333333333333",
                  "cantidad": 2,
                  "precioUnitario": 12.50
                }
              ],
              "total": 25.00,
              "estado": "Pendiente",
              "fechaCreacion": "2026-11-09T16:00:00Z"
            }
            """);

    operation.Responses["422"].Content!["application/problem+json"].Example =
        JsonNode.Parse("""
            {
              "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
              "title": "Unprocessable Entity",
              "status": 422,
              "detail": "Stock insuficiente para el producto 33333333-3333-4333-8333-333333333333.",
              "instance": "/api/orders",
              "errorCode": "ORD-005",
              "errorMessage": "Stock insuficiente para el producto 33333333-3333-4333-8333-333333333333.",
              "correlationId": "demo-orders-422"
            }
            """);
  }
}
