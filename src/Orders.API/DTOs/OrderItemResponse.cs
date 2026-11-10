namespace Orders.API.DTOs;

/// <summary>Detalle de un producto de la orden devuelto por la API.</summary>
public class OrderItemResponse
{
    /// <summary>Identificador del producto.</summary>
    public Guid ProductoId { get; set; }
    /// <summary>Cantidad de unidades incluidas en la orden.</summary>
    public int Cantidad { get; set; }
    /// <summary>Precio unitario registrado al crear la orden.</summary>
    public decimal PrecioUnitario { get; set; }
}