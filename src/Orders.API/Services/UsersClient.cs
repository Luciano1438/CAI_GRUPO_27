using System.Net;

namespace Orders.API.Services;

public class UsersClient
{
    private readonly HttpClient _client;

    public UsersClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> ExisteAsync(
        Guid usuarioId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        using var solicitud = new HttpRequestMessage(
            HttpMethod.Get, $"api/users/{usuarioId}");

        solicitud.Headers.Add("X-Correlation-Id", correlationId);

        using var respuesta = await _client.SendAsync(
            solicitud, cancellationToken);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        respuesta.EnsureSuccessStatusCode();
        return true;
    }
}