namespace Orders.API.DTOs;

/// <summary>Datos solicitados para cambiar el estado de una orden.</summary>
public class UpdateOrderStatusRequest
{
    /// <summary>Nuevo estado solicitado. Debe respetar las transiciones permitidas.</summary>
    public string Estado { get; set; } = string.Empty;
}