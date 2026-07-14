namespace FiolinOne.Application.Commerce;

public interface ICommerceService
{
    Task<CommerceHomeDto> GetHomeAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CommerceCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CommerceProductDto>> GetProductsAsync(string? search, CancellationToken cancellationToken);
    Task<CommerceProductDetailDto?> GetProductAsync(string slug, CancellationToken cancellationToken);
    Task<IReadOnlyList<CommerceMenuDto>> GetMenuAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CommerceBannerDto>> GetBannersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CommerceSliderDto>> GetSlidersAsync(CancellationToken cancellationToken);
    Task<CommerceSettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
}
