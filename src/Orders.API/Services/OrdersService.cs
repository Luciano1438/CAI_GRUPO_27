using Orders.API.DTOs;
using Orders.API.Exceptions;

namespace Orders.API.Services;

public class OrdersService
{
    public void ValidateCreateRequest(CreateOrderRequest? request)
    {
        if (request == null)
        {
            throw new OrderValidationException("El pedido debe incluir los datos de la orden.");
        }

        if (request.UsuarioId == Guid.Empty)
        {
            throw new OrderValidationException("Indicá el ID del usuario.");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new OrderValidationException("La orden debe tener al menos un ítem.");
        }

        foreach (var item in request.Items)
        {
            if (item == null || item.ProductoId == Guid.Empty)
            {
                throw new OrderValidationException("Cada ítem debe tener un ID de producto.");
            }

            if (item.Cantidad <= 0)
            {
                throw new OrderValidationException("La cantidad de cada ítem debe ser mayor a cero.");
            }
        }
    }
}