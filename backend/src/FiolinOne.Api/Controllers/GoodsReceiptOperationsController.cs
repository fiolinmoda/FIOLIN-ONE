using FiolinOne.Application.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/v2/goods-receipt")]
[Produces("application/json")]
public sealed class GoodsReceiptOperationsController(IGoodsReceiptOperationService goodsReceiptOperationService) : ControllerBase
{
    /// <summary>
    /// Finds a product variant by its barcode for V2 goods receipt operations.
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    [ProducesResponseType(typeof(GoodsReceiptVariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var variant = await goodsReceiptOperationService.FindVariantByBarcodeAsync(barcode, cancellationToken);

        return variant is null ? NotFound(new { message = "Barkod sistemde bulunamadı." }) : Ok(variant);
    }

    /// <summary>
    /// Gets a product variant for V2 goods receipt operations.
    /// </summary>
    [HttpGet("variants/{productVariantId:guid}")]
    [ProducesResponseType(typeof(GoodsReceiptVariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVariant(Guid productVariantId, CancellationToken cancellationToken)
    {
        var variant = await goodsReceiptOperationService.GetVariantAsync(productVariantId, cancellationToken);

        return variant is null ? NotFound(new { message = "Ürün varyantı bulunamadı." }) : Ok(variant);
    }

    /// <summary>
    /// Saves a V2 goods receipt transaction and increases product stock.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GoodsReceiptOperationResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateGoodsReceiptOperationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await goodsReceiptOperationService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetVariant), new { productVariantId = result.ProductVariantId }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
