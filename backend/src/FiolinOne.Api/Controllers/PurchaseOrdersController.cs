using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/purchase-orders")]
[Produces("application/json")]
public sealed class PurchaseOrdersController(IPurchasingService purchasingService) : ControllerBase
{
    /// <summary>
    /// Gets purchase orders with pagination, filtering, and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var orders = await purchasingService.GetPurchaseOrdersAsync(
            new PurchasingQuery(search, status, sortBy, sortDirection, page, pageSize),
            cancellationToken);

        return Ok(orders);
    }

    /// <summary>
    /// Gets a purchase order by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseOrder(Guid id, CancellationToken cancellationToken)
    {
        var order = await purchasingService.GetPurchaseOrderAsync(id, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Creates a purchase order and its item lines.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePurchaseOrder(
        [FromBody] CreatePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await purchasingService.CreatePurchaseOrderAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = order.Id }, order);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a purchase order and replaces its item lines.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePurchaseOrder(
        Guid id,
        [FromBody] UpdatePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await purchasingService.UpdatePurchaseOrderAsync(id, request, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Soft deletes a purchase order.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePurchaseOrder(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await purchasingService.DeletePurchaseOrderAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
