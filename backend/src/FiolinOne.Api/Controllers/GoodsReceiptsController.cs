using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Purchasing;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/goods-receipts")]
[Produces("application/json")]
public sealed class GoodsReceiptsController(IPurchasingService purchasingService) : ControllerBase
{
    /// <summary>
    /// Gets goods receipts with pagination, filtering, and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GoodsReceiptDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGoodsReceipts(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var receipts = await purchasingService.GetGoodsReceiptsAsync(
            new PurchasingQuery(search, status, sortBy, sortDirection, page, pageSize),
            cancellationToken);

        return Ok(receipts);
    }

    /// <summary>
    /// Gets a goods receipt by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GoodsReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGoodsReceipt(Guid id, CancellationToken cancellationToken)
    {
        var receipt = await purchasingService.GetGoodsReceiptAsync(id, cancellationToken);

        return receipt is null ? NotFound() : Ok(receipt);
    }

    /// <summary>
    /// Creates a goods receipt and its item lines.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GoodsReceiptDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGoodsReceipt(
        [FromBody] CreateGoodsReceiptRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var receipt = await purchasingService.CreateGoodsReceiptAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetGoodsReceipt), new { id = receipt.Id }, receipt);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a goods receipt and replaces its item lines.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GoodsReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateGoodsReceipt(
        Guid id,
        [FromBody] UpdateGoodsReceiptRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var receipt = await purchasingService.UpdateGoodsReceiptAsync(id, request, cancellationToken);

            return receipt is null ? NotFound() : Ok(receipt);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Soft deletes a goods receipt.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGoodsReceipt(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await purchasingService.DeleteGoodsReceiptAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
