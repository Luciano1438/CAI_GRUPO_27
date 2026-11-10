using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
[ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status500InternalServerError, "application/problem+json")]
public class OrdersController : ControllerBase
{
    private readonly OrdersService _service;

    public OrdersController(OrdersService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista las órdenes, con filtro opcional por usuario.
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario por el cual filtrar.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status400BadRequest, "application/problem+json")]
    public async Task<ActionResult<List<OrderResponse>>> Listar(
        [FromQuery] Guid? usuarioId)
    {
        List<OrderResponse> ordenes = await _service.ListarAsync(usuarioId);

        return Ok(ordenes);
    }

    /// <summary>
    /// Obtiene una orden por su identificador.
    /// </summary>
    /// <response code="404">La orden no existe (ORD-001).</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<OrderResponse>> Buscar(Guid id)
    {
        OrderResponse orden = await _service.BuscarAsync(id);

        return Ok(orden);
    }

    /// <summary>
    /// Crea una orden validando el usuario, los productos y el stock.
    /// </summary>
    /// <remarks>
    /// Agrupa los productos repetidos y calcula el total con los precios
    /// obtenidos de Products.API.
    /// </remarks>
    /// <response code="201">Orden creada. Location indica la URL de consulta.</response>
    /// <response code="400">Los datos enviados son inválidos (ORD-002).</response>
    /// <response code="404">El usuario o un producto no existe (ORD-003 u ORD-004).</response>
    /// <response code="415">El tipo de contenido no es compatible. Enviar application/json.</response>
    /// <response code="422">Un producto no tiene stock suficiente (ORD-005).</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status415UnsupportedMediaType, "application/problem+json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status422UnprocessableEntity, "application/problem+json")]
    public async Task<ActionResult<OrderResponse>> Crear(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        OrderResponse orden = await _service.CrearAsync(
            request, HttpContext.TraceIdentifier, cancellationToken);

        return CreatedAtAction(nameof(Buscar), new { id = orden.Id }, orden);
    }

    /// <summary>
    /// Cambia el estado de una orden.
    /// </summary>
    /// <response code="400">Los datos enviados son inválidos (ORD-002).</response>
    /// <response code="404">La orden no existe (ORD-001).</response>
    /// <response code="409">El cambio de estado no está permitido (ORD-006).</response>
    /// <response code="415">El tipo de contenido no es compatible. Enviar application/json.</response>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(UpdateOrderStatusResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status409Conflict, "application/problem+json")]
    [ProducesResponseType(typeof(OrderErrorResponse), StatusCodes.Status415UnsupportedMediaType, "application/problem+json")]
    public async Task<ActionResult<UpdateOrderStatusResponse>> CambiarEstado(
        Guid id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var respuesta = await _service.CambiarEstadoAsync(id, request);

        return Ok(respuesta);
    }
}