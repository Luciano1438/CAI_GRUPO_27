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

    public Task<List<Order>> GetAllAsync(Guid? usuarioId)
{
    return BuscarAsync(usuarioId, null);
}

public async Task<Order?> GetByIdAsync(Guid id)
{
    List<Order> ordenes = await BuscarAsync(null, id);

    if (ordenes.Count == 0)
    {
        return null;
    }

    return ordenes[0];
}

private async Task<List<Order>> BuscarAsync(Guid? usuarioId, Guid? id)
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
             AND (@Id IS NULL OR id = @Id)
            ORDER BY fecha_creacion DESC, id;
            """,
            new
            {
            UsuarioId = usuarioId?.ToString(),
            Id = id?.ToString()
            },
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

   public async Task GuardarAsync(Order orden)
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

    string orderId = orden.Id.ToString();

    await connection.ExecuteAsync(
        """
        INSERT INTO orders (id, usuario_id, total, estado, fecha_creacion)
        VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion);
        """,
        new
        {
            Id = orderId,
            UsuarioId = orden.UsuarioId.ToString(),
            Total = orden.Total.ToString(CultureInfo.InvariantCulture),
            Estado = orden.Estado,
            FechaCreacion = orden.FechaCreacion.ToString(
                "O", CultureInfo.InvariantCulture)
        },
        transaction: transaction);

    foreach (OrderItem item in orden.Items)
    {
        await connection.ExecuteAsync(
            """
            INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario)
            VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario);
            """,
            new
            {
                OrderId = orderId,
                ProductoId = item.ProductoId.ToString(),
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario.ToString(
                    CultureInfo.InvariantCulture)
            },
            transaction: transaction);
    }

    transaction.Commit();
}
public async Task<bool> ActualizarEstadoAsync(
    Guid id, string estadoActual, string nuevoEstado, DateTime fechaActualizacion)
{
    string? connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Falta configurar la conexion de Orders.");
    }

    using var connection = new SqliteConnection(connectionString);

    int filas = await connection.ExecuteAsync(
        """
        UPDATE orders
        SET estado = @NuevoEstado,
            fecha_actualizacion = @FechaActualizacion
        WHERE id = @Id AND estado = @EstadoActual;
        """,
        new
        {
            Id = id.ToString(),
            EstadoActual = estadoActual,
            NuevoEstado = nuevoEstado,
            FechaActualizacion = fechaActualizacion.ToString(
                "O", CultureInfo.InvariantCulture)
        });

    return filas == 1;
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