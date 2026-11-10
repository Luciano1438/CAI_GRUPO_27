namespace Orders.API.DTOs;

/// <summary>Datos necesarios para crear una orden.</summary>
public class CreateOrderRequest
{
    /// <summary>Identificador del usuario que realiza la orden.</summary>
    public Guid UsuarioId { get; set; }
    /// <summary>Productos solicitados. La lista debe contener al menos un elemento.</summary>
    public List<CreateOrderItemRequest> Items { get; set; } = new List<CreateOrderItemRequest>();
}
