using System.Globalization;
using Dapper;
using Microsoft.Data.Sqlite;
using Orders.API.Models;

namespace Orders.API.Data;

public class OrderRepository
{
    private readonly IConfiguration _configuration;

    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Order>> GetAllAsync(Guid? usuarioId)
    {
        string? connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Falta configurar la conexion de Orders.");
        }

        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        var filas = await connection.QueryAsync<OrdenGuardada>(
            """
            SELECT id AS Id,
                   usuario_id AS UsuarioId,
                   total AS Total,
                   estado AS Estado,
                   fecha_creacion AS FechaCreacion
            FROM orders
            WHERE (@UsuarioId IS NULL OR usuario_id = @UsuarioId)
            ORDER BY fecha_creacion DESC, id;
            """,
            new { UsuarioId = usuarioId?.ToString() },
            transaction: transaction);

        List<Order> ordenes = new List<Order>();

        foreach (OrdenGuardada fila in filas)
        {
            var orden = new Order
            {
                Id = Guid.Parse(fila.Id),
                UsuarioId = Guid.Parse(fila.UsuarioId),
                Total = decimal.Parse(
                    fila.Total, CultureInfo.InvariantCulture),
                Estado = fila.Estado,
                FechaCreacion = DateTime.Parse(
                    fila.FechaCreacion,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal)
            };

            var itemsGuardados = await connection.QueryAsync<ItemGuardado>(
                """
                SELECT producto_id AS ProductoId,
                       cantidad AS Cantidad,
                       precio_unitario AS PrecioUnitario
                FROM order_items
                WHERE order_id = @OrderId
                ORDER BY id;
                """,
                new { OrderId = fila.Id },
                transaction: transaction);

            foreach (ItemGuardado item in itemsGuardados)
            {
                orden.Items.Add(new OrderItem
                {
                    ProductoId = Guid.Parse(item.ProductoId),
                    Cantidad = item.Cantidad,
                    PrecioUnitario = decimal.Parse(
                        item.PrecioUnitario,
                        CultureInfo.InvariantCulture)
                });
            }

            ordenes.Add(orden);
        }

        transaction.Commit();

        return ordenes;
    }

    private class OrdenGuardada
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
    }

    private class ItemGuardado
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string PrecioUnitario { get; set; } = string.Empty;
    }
}