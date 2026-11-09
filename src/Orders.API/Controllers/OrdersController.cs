using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrdersService _service;

    public OrdersController(OrdersService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista las ordenes, con filtro opcional por usuario.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderResponse>>> Listar(
        [FromQuery] Guid? usuarioId)
    {
        List<OrderResponse> ordenes = await _service.ListarAsync(usuarioId);

        return Ok(ordenes);
    }
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Buscar(Guid id)
    {
      OrderResponse orden = await _service.BuscarAsync(id);

      return Ok(orden);
    }
}