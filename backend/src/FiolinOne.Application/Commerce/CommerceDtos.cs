namespace FiolinOne.Application.Commerce;

public sealed record CommerceCategoryDto(Guid Id, string Name, string Code, string Slug, int SortOrder);

public sealed record CommerceProductDto(
    Guid Id,
    string ModelCode,
    string Slug,
    string ProductName,
    string? Category,
    string? ImageUrl,
    decimal Price,
    int Stock);

public sealed record CommerceProductDetailDto(
    Guid Id,
    string ModelCode,
    string Slug,
    string ProductName,
    string? Category,
    string? ImageUrl,
    decimal Price,
    int Stock,
    IReadOnlyList<CommerceVariantDto> Variants,
    CommerceSeoDto Seo);

public sealed record CommerceVariantDto(string Color, string Size, string Barcode, int Stock, decimal Price);

public sealed record CommerceMenuDto(string Title, string Url, string Location, int SortOrder);

public sealed record CommerceBannerDto(string Title, string? Subtitle, string ImageUrl, string? LinkUrl, string Placement);

public sealed record CommerceSliderDto(string Title, string? Subtitle, string ImageUrl, string? LinkUrl);

public sealed record CommerceSettingsDto(
    string Primary,
    string Secondary,
    string Typography,
    bool DarkModeEnabled,
    IReadOnlyDictionary<string, string> Values);

public sealed record CommerceSeoDto(
    string MetaTitle,
    string MetaDescription,
    string? Canonical,
    string? OpenGraphTitle,
    string? OpenGraphDescription,
    string? TwitterCard,
    string? SchemaJson);

public sealed record CommerceHomeDto(
    IReadOnlyList<CommerceSliderDto> HeroSlider,
    IReadOnlyList<CommerceCategoryDto> Categories,
    IReadOnlyList<CommerceProductDto> NewSeason,
    IReadOnlyList<CommerceProductDto> BestSellers,
    IReadOnlyList<CommerceProductDto> NewArrivals,
    IReadOnlyList<CommerceBannerDto> Campaigns,
    CommerceSeoDto Seo);
