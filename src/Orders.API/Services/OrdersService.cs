using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Data;
using Orders.API.Models;

namespace Orders.API.Services;

public class OrdersService
{
  private readonly OrderRepository _repository;

public OrdersService(OrderRepository repository)
{
    _repository = repository;
}

public async Task<List<OrderResponse>> ListarAsync(Guid? usuarioId)
{
    List<Order> ordenes = await _repository.GetAllAsync(usuarioId);
    List<OrderResponse> respuesta = new List<OrderResponse>();

    foreach (Order orden in ordenes)
    {
        respuesta.Add(ConvertirAResponse(orden));
    }

    return respuesta;
}

public async Task<OrderResponse> BuscarAsync(Guid id)
{
    Order? orden = await _repository.GetByIdAsync(id);

    if (orden == null)
    {
        throw new OrderNotFoundException();
    }

    return ConvertirAResponse(orden);
}

private static OrderResponse ConvertirAResponse(Order orden)
{
    var respuesta = new OrderResponse
    {
        Id = orden.Id,
        UsuarioId = orden.UsuarioId,
        Total = orden.Total,
        Estado = orden.Estado,
        FechaCreacion = orden.FechaCreacion
    };

    foreach (OrderItem item in orden.Items)
    {
        respuesta.Items.Add(new OrderItemResponse
        {
            ProductoId = item.ProductoId,
            Cantidad = item.Cantidad,
            PrecioUnitario = item.PrecioUnitario
        });
    }

    return respuesta;
}
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