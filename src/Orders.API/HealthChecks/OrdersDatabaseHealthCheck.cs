using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Orders.API.HealthChecks;

public class OrdersDatabaseHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public OrdersDatabaseHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        string? connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Unhealthy("Falta configurar la base de Orders.");
        }

        try
        {
            var opciones = new SqliteConnectionStringBuilder(connectionString)
            {
                Mode = SqliteOpenMode.ReadWrite,
                DefaultTimeout = 3
            };

            using var connection = new SqliteConnection(opciones.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition(
    """
    SELECT o.id, o.usuario_id, o.total, o.estado,
           o.fecha_creacion, o.fecha_actualizacion,
           i.id, i.order_id, i.producto_id, i.cantidad, i.precio_unitario
    FROM orders AS o
    CROSS JOIN order_items AS i
    LIMIT 0;
    """,
    cancellationToken: cancellationToken));

            return HealthCheckResult.Healthy("La base de Orders está disponible.");
        }
        catch (SqliteException)
        {
            return HealthCheckResult.Unhealthy("La base de Orders no está disponible.");
        }
    }
}