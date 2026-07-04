using FiolinOne.Application.Products.Variants;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/products/{productId:guid}/variants")]
[Produces("application/json")]
public sealed class ProductVariantsController(IProductVariantService productVariantService) : ControllerBase
{
    /// <summary>
    /// Gets variants for a product.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductVariantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVariants(Guid productId, CancellationToken cancellationToken)
    {
        var variants = await productVariantService.GetVariantsAsync(productId, cancellationToken);

        return Ok(variants);
    }

    /// <summary>
    /// Gets a product variant by identifier.
    /// </summary>
    [HttpGet("{variantId:guid}")]
    [ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVariant(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        var variant = await productVariantService.GetVariantAsync(productId, variantId, cancellationToken);

        return variant is null ? NotFound() : Ok(variant);
    }

    /// <summary>
    /// Creates a sellable product variant.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateVariant(
        Guid productId,
        [FromBody] CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var variant = await productVariantService.CreateVariantAsync(productId, request, cancellationToken);

            return variant is null
                ? NotFound()
                : CreatedAtAction(nameof(GetVariant), new { productId, variantId = variant.Id }, variant);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a sellable product variant.
    /// </summary>
    [HttpPut("{variantId:guid}")]
    [ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateVariant(
        Guid productId,
        Guid variantId,
        [FromBody] UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var variant = await productVariantService.UpdateVariantAsync(productId, variantId, request, cancellationToken);

            return variant is null ? NotFound() : Ok(variant);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Deletes a product variant.
    /// </summary>
    [HttpDelete("{variantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVariant(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        var deleted = await productVariantService.DeleteVariantAsync(productId, variantId, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
