namespace Orders.API.Models;

/// <summary>Producto incluido en una orden.</summary>
public class OrderItem
{
    /// <summary>Identificador del producto.</summary>
    public Guid ProductoId { get; set; }
    /// <summary>Cantidad de unidades solicitadas.</summary>
    public int Cantidad { get; set; }
    /// <summary>Precio unitario registrado al crear la orden.</summary>
    public decimal PrecioUnitario { get; set; }
}