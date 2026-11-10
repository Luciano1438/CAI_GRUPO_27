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
        && operation.RequestBody?.Content is not null
        && (context.MethodInfo.Name == nameof(OrdersController.Crear)
            || context.MethodInfo.Name == nameof(OrdersController.CambiarEstado)))
    {
      string solicitud = context.MethodInfo.Name == nameof(OrdersController.Crear)
          ? """
            {
              "usuarioId": "22222222-2222-4222-8222-222222222222",
              "items": [
                { "productoId": "33333333-3333-4333-8333-333333333333", "cantidad": 2 }
              ]
            }
            """
          : """{"estado":"Confirmada"}""";
      foreach (var medioSolicitud in operation.RequestBody.Content.Values)
      {
        medioSolicitud.Example = JsonNode.Parse(solicitud);
      }
    }

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

      var ruta = context.MethodInfo.Name switch
      {
        nameof(OrdersController.Buscar) => $"/api/orders/{id}",
        nameof(OrdersController.CambiarEstado) => $"/api/orders/{id}/status",
        _ => "/api/orders"
      };

      AgregarEjemploError("500", "Internal Server Error", "ORD-007",
          "Error interno al procesar la orden.", ruta);

      if (context.MethodInfo.Name is nameof(OrdersController.Crear)
    or nameof(OrdersController.CambiarEstado))
      {
        AgregarEjemploError("415", "Unsupported Media Type", "ORD-002",
            "El tipo de contenido no es compatible. Enviá application/json.", ruta);
      }

      switch (context.MethodInfo.Name)
      {
        case nameof(OrdersController.Listar):
          AgregarEjemploError("400", "Bad Request", "ORD-002",
              "Los datos de la orden son inválidos.", ruta);
          break;

        case nameof(OrdersController.Crear):
          AgregarEjemploError("400", "Bad Request", "ORD-002",
              "Indicá el ID del usuario.", ruta);
          AgregarEjemploError("404", "Not Found", "ORD-003",
              "Usuario no encontrado.", ruta, "usuarioNoEncontrado");
          AgregarEjemploError("404", "Not Found", "ORD-004",
              "Producto no encontrado.", ruta, "productoNoEncontrado");
          break;

        case nameof(OrdersController.Buscar):
          AgregarEjemploError("404", "Not Found", "ORD-001",
              "Orden no encontrada.", ruta);
          break;

        case nameof(OrdersController.CambiarEstado):
          AgregarEjemploError("400", "Bad Request", "ORD-002",
              "El estado indicado no es válido.", ruta);
          AgregarEjemploError("404", "Not Found", "ORD-001",
              "Orden no encontrada.", ruta);
          AgregarEjemploError("409", "Conflict", "ORD-006",
              "No se puede cambiar de Confirmada a Pendiente.", ruta);
          break;
      }
    }

    void AgregarEjemploError(string codigoHttp, string titulo, string codigoError,
    string mensaje, string ruta, string? nombre = null)
    {
      if (operation.Responses is null
    || !operation.Responses.TryGetValue(codigoHttp, out var error)
    || error is null
    || error.Content is null
    || !error.Content.TryGetValue("application/problem+json", out var json)
    || json is null)
      {
        return;
      }

      string tipo = codigoHttp switch
      {
        "400" => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        "404" => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        "409" => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        "415" => "https://tools.ietf.org/html/rfc9110#section-15.5.16",
        "500" => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        _ => "about:blank"
      };

      var ejemplo = new JsonObject
      {
        ["type"] = tipo,
        ["title"] = titulo,
        ["status"] = int.Parse(codigoHttp),
        ["detail"] = mensaje,
        ["instance"] = ruta,
        ["errorCode"] = codigoError,
        ["errorMessage"] = mensaje,
        ["correlationId"] = $"demo-orders-{codigoHttp}"
      };

      if (nombre is null)
      {
        json.Example = ejemplo;
        return;
      }

      json.Example = null;
      json.Examples ??= new Dictionary<string, IOpenApiExample>();
      json.Examples[nombre] = new OpenApiExample
      {
        Summary = mensaje,
        Value = ejemplo
      };
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
