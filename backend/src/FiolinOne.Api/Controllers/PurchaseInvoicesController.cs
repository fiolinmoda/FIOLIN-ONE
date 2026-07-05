using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/purchase-invoices")]
[Produces("application/json")]
public sealed class PurchaseInvoicesController(IPurchasingService purchasingService) : ControllerBase
{
    /// <summary>
    /// Gets purchase invoices with pagination, filtering, and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PurchaseInvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseInvoices(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var invoices = await purchasingService.GetPurchaseInvoicesAsync(
            new PurchasingQuery(search, status, sortBy, sortDirection, page, pageSize),
            cancellationToken);

        return Ok(invoices);
    }

    /// <summary>
    /// Gets a purchase invoice by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseInvoice(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await purchasingService.GetPurchaseInvoiceAsync(id, cancellationToken);

        return invoice is null ? NotFound() : Ok(invoice);
    }

    /// <summary>
    /// Creates a purchase invoice and its item lines.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseInvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePurchaseInvoice(
        [FromBody] CreatePurchaseInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await purchasingService.CreatePurchaseInvoiceAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetPurchaseInvoice), new { id = invoice.Id }, invoice);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a purchase invoice and replaces its item lines.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePurchaseInvoice(
        Guid id,
        [FromBody] UpdatePurchaseInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await purchasingService.UpdatePurchaseInvoiceAsync(id, request, cancellationToken);

            return invoice is null ? NotFound() : Ok(invoice);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Soft deletes a purchase invoice.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePurchaseInvoice(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await purchasingService.DeletePurchaseInvoiceAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
