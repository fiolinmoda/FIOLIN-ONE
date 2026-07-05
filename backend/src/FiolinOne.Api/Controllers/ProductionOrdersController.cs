using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Production;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/production-orders")]
[Produces("application/json")]
public sealed class ProductionOrdersController(IProductionService productionService) : ControllerBase
{
    /// <summary>Gets production dashboard counts.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ProductionDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        return Ok(await productionService.GetDashboardAsync(cancellationToken));
    }

    /// <summary>Gets production orders with pagination, filtering, and sorting.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductionOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        return Ok(await productionService.GetOrdersAsync(new ProductionQuery(search, status, sortBy, sortDirection, page, pageSize), cancellationToken));
    }

    /// <summary>Gets a production order by identifier.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductionOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var order = await productionService.GetOrderAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>Creates a production order.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductionOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateProductionOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await productionService.CreateOrderAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>Updates a production order.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductionOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateProductionOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await productionService.UpdateOrderAsync(id, request, cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>Soft deletes a production order.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await productionService.DeleteOrderAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>Sends a production order to ironing and packaging.</summary>
    [HttpPost("{id:guid}/ironing-packaging")]
    [ProducesResponseType(typeof(ProductionOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendToIroningPackaging(Guid id, CancellationToken cancellationToken)
    {
        var order = await productionService.SendToIroningPackagingAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }
}
