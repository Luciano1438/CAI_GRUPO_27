using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Data;
using Orders.API.Models;

namespace Orders.API.Services;

public class OrdersService
{
  private readonly OrderRepository _repository;
private readonly UsersClient _usersClient;
private readonly ProductsClient _productsClient;

public OrdersService(
    OrderRepository repository,
    UsersClient usersClient,
    ProductsClient productsClient)
{
    _repository = repository;
    _usersClient = usersClient;
    _productsClient = productsClient;
}

public async Task ValidarUsuarioAsync(
    Guid usuarioId,
    string correlationId,
    CancellationToken cancellationToken)
{
    bool existe = await _usersClient.ExisteAsync(
        usuarioId, correlationId, cancellationToken);

    if (!existe)
    {
        throw new OrderNotFoundException("ORD-003", "Usuario no encontrado.");
    }
}

private async Task<List<OrderItem>> PrepararItemsAsync(
    List<CreateOrderItemRequest> items,
    string correlationId,
    CancellationToken cancellationToken)
{
    var cantidades = new Dictionary<Guid, long>();

    foreach (CreateOrderItemRequest item in items)
    {
        cantidades.TryGetValue(item.ProductoId, out long cantidadAcumulada);
        cantidades[item.ProductoId] = cantidadAcumulada + item.Cantidad;
    }

    var itemsPreparados = new List<OrderItem>();

    foreach (var par in cantidades)
    {
        Guid productoId = par.Key;
        long cantidad = par.Value;

        var producto = await _productsClient.BuscarAsync(
            productoId, correlationId, cancellationToken);

        if (cantidad > producto.Stock)
        {
            throw new InsufficientStockException(productoId);
        }

        itemsPreparados.Add(new OrderItem
        {
            ProductoId = productoId,
            Cantidad = (int)cantidad,
            PrecioUnitario = producto.Precio
        });
    }

    return itemsPreparados;
}

private async Task<Order> PrepararOrdenAsync(
    CreateOrderRequest request,
    string correlationId,
    CancellationToken cancellationToken)
{
    ValidateCreateRequest(request);

    await ValidarUsuarioAsync(
        request.UsuarioId, correlationId, cancellationToken);

    List<OrderItem> items = await PrepararItemsAsync(
        request.Items, correlationId, cancellationToken);

    decimal total = 0m;

    foreach (OrderItem item in items)
    {
        total += item.Cantidad * item.PrecioUnitario;
    }

    return new Order
    {
        Id = Guid.NewGuid(),
        UsuarioId = request.UsuarioId,
        Items = items,
        Total = total,
        Estado = "Pendiente",
        FechaCreacion = DateTime.UtcNow
    };
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