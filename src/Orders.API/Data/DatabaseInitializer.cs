using Dapper;
using Microsoft.Data.Sqlite;

namespace Orders.API.Data;

public class DatabaseInitializer
{
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Initialize()
    {
        string? connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Falta configurar la conexion de Orders.");
        }

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS orders (
                id TEXT PRIMARY KEY NOT NULL,
                usuario_id TEXT NOT NULL,
                total TEXT NOT NULL,
                estado TEXT NOT NULL,
                fecha_creacion TEXT NOT NULL,
                fecha_actualizacion TEXT
            );

            CREATE TABLE IF NOT EXISTS order_items (
                id INTEGER PRIMARY KEY,
                order_id TEXT NOT NULL,
                producto_id TEXT NOT NULL,
                cantidad INTEGER NOT NULL CHECK (cantidad > 0),
                precio_unitario TEXT NOT NULL,
                FOREIGN KEY (order_id) REFERENCES orders(id)
            );

            CREATE INDEX IF NOT EXISTS ix_orders_usuario
                ON orders(usuario_id);

            CREATE INDEX IF NOT EXISTS ix_order_items_order
                ON order_items(order_id);
            """, transaction: transaction);

        transaction.Commit();
    }
}