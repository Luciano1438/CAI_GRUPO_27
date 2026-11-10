namespace Orders.API.DTOs;

/// <summary>Información de una orden devuelta por la API.</summary>
public class OrderResponse
{
    /// <summary>Identificador de la orden.</summary>
    public Guid Id { get; set; }
    /// <summary>Identificador del usuario que realizó la orden.</summary>
    public Guid UsuarioId { get; set; }
    /// <summary>Detalle de los productos incluidos en la orden.</summary>
    public List<OrderItemResponse> Items { get; set; } =
        new List<OrderItemResponse>();

    /// <summary>Importe total de la orden.</summary>
    public decimal Total { get; set; }
    /// <summary>Estado actual de la orden.</summary>
    public string Estado { get; set; } = string.Empty;
    /// <summary>Fecha y hora de creación de la orden.</summary>
    public DateTime FechaCreacion { get; set; }
}