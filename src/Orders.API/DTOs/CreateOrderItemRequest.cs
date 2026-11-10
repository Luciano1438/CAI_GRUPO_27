namespace Orders.API.DTOs;

/// <summary>Datos de un producto solicitado al crear una orden.</summary>
public class CreateOrderItemRequest
{
    /// <summary>Identificador del producto solicitado.</summary>
    public Guid ProductoId { get; set; }
    /// <summary>Cantidad de unidades solicitadas; debe ser mayor que cero.</summary>
    public int Cantidad { get; set; }
}