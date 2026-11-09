using System.Net;
using System.Net.Http.Json;
using Orders.API.DTOs;
using Orders.API.Exceptions;

namespace Orders.API.Services;

public class ProductsClient
{
    private readonly HttpClient _client;

    public ProductsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ProductoConsultaResponse> BuscarAsync(
        Guid productoId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        using var solicitud = new HttpRequestMessage(
            HttpMethod.Get, $"api/products/{productoId}");

        solicitud.Headers.Add("X-Correlation-Id", correlationId);

        using var respuesta = await _client.SendAsync(
            solicitud, cancellationToken);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            throw new OrderNotFoundException(
                "ORD-004", "Producto no encontrado.");
        }

        respuesta.EnsureSuccessStatusCode();

        var producto = await respuesta.Content
            .ReadFromJsonAsync<ProductoConsultaResponse>(cancellationToken);

        if (producto == null || producto.Id != productoId
            || producto.Precio <= 0 || producto.Stock < 0)
        {
            throw new InvalidOperationException(
                "Products devolvió datos de producto inválidos.");
        }

        return producto;
    }
}