namespace Orders.API.DTOs;

/// <summary>Resultado del cambio de estado de una orden.</summary>
public class UpdateOrderStatusResponse
{
    /// <summary>Identificador de la orden actualizada.</summary>
    public Guid Id { get; set; }
    /// <summary>Estado de la orden luego del cambio.</summary>
    public string Estado { get; set; } = string.Empty;
    /// <summary>Fecha y hora del cambio de estado.</summary>
    public DateTime FechaActualizacion { get; set; }
}