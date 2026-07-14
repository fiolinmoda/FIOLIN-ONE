using FiolinOne.Application.Commerce;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

/// <summary>
/// Provides readonly commerce storefront data from ERP products and CMS content.
/// </summary>
[ApiController]
[Route("commerce")]
public sealed class CommerceController(ICommerceService commerceService) : ControllerBase
{
    /// <summary>
    /// Gets the storefront home page composition.
    /// </summary>
    [HttpGet("home")]
    [ProducesResponseType(typeof(CommerceHomeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CommerceHomeDto>> GetHome(CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetHomeAsync(cancellationToken));
    }

    /// <summary>
    /// Gets active ERP categories for the storefront.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<CommerceCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommerceCategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetCategoriesAsync(cancellationToken));
    }

    /// <summary>
    /// Gets active storefront products from ERP product cards.
    /// </summary>
    [HttpGet("products")]
    [ProducesResponseType(typeof(IReadOnlyList<CommerceProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommerceProductDto>>> GetProducts(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetProductsAsync(search, cancellationToken));
    }

    /// <summary>
    /// Gets one storefront product by slug.
    /// </summary>
    [HttpGet("product/{slug}")]
    [ProducesResponseType(typeof(CommerceProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommerceProductDetailDto>> GetProduct(
        string slug,
        CancellationToken cancellationToken)
    {
        var product = await commerceService.GetProductAsync(slug, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Gets CMS menus for storefront header and footer.
    /// </summary>
    [HttpGet("menu")]
    [ProducesResponseType(typeof(IReadOnlyList<CommerceMenuDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommerceMenuDto>>> GetMenu(CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetMenuAsync(cancellationToken));
    }

    /// <summary>
    /// Gets CMS banners for storefront placements.
    /// </summary>
    [HttpGet("banner")]
    [ProducesResponseType(typeof(IReadOnlyList<CommerceBannerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommerceBannerDto>>> GetBanner(CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetBannersAsync(cancellationToken));
    }

    /// <summary>
    /// Gets CMS sliders for storefront hero areas.
    /// </summary>
    [HttpGet("slider")]
    [ProducesResponseType(typeof(IReadOnlyList<CommerceSliderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommerceSliderDto>>> GetSlider(CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetSlidersAsync(cancellationToken));
    }

    /// <summary>
    /// Gets CMS theme and storefront settings.
    /// </summary>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(CommerceSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CommerceSettingsDto>> GetSettings(CancellationToken cancellationToken)
    {
        return Ok(await commerceService.GetSettingsAsync(cancellationToken));
    }
}
