using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Sales;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/sales-orders")]
[Produces("application/json")]
public sealed class SalesOrdersController(ISalesService salesService) : ControllerBase
{
    /// <summary>
    /// Satış siparişlerini sayfalama, arama, filtreleme ve sıralama ile listeler.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SalesOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var orders = await salesService.GetOrdersAsync(
            new SalesQuery(search, status, sortBy, sortDirection, page, pageSize),
            cancellationToken);

        return Ok(orders);
    }

    /// <summary>
    /// Satış siparişini kimliğine göre getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SalesOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var order = await salesService.GetOrderAsync(id, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Satış siparişi oluşturur. Tamamlanan siparişler ürün varyant stoğunu düşürür.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SalesOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateSalesOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await salesService.CreateOrderAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Satış siparişini günceller. Siparişi tamamlandı durumuna almak ürün varyant stoğunu düşürür.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SalesOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateSalesOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await salesService.UpdateOrderAsync(id, request, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Stoğu etkilememiş satış siparişini pasif olarak siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await salesService.DeleteOrderAsync(id, cancellationToken);

            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
