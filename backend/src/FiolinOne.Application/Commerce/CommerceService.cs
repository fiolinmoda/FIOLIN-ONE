using System.Globalization;
using System.Text;
using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.Commerce;

public sealed class CommerceService(ICommerceRepository commerceRepository) : ICommerceService
{
    public async Task<CommerceHomeDto> GetHomeAsync(CancellationToken cancellationToken)
    {
        var products = await GetProductsAsync(null, cancellationToken);
        var categories = await GetCategoriesAsync(cancellationToken);
        var banners = await GetBannersAsync(cancellationToken);
        var sliders = await GetSlidersAsync(cancellationToken);
        var seo = await GetSeoAsync("/", "FIOLIN", "FIOLIN yeni sezon kadın giyim koleksiyonu.", cancellationToken);

        return new CommerceHomeDto(
            sliders,
            categories,
            products.Take(8).ToList(),
            products.OrderByDescending(product => product.Stock).Take(8).ToList(),
            products.Skip(8).Take(8).ToList(),
            banners.Where(banner => banner.Placement.Equals("campaign", StringComparison.OrdinalIgnoreCase)).ToList(),
            seo);
    }

    public async Task<IReadOnlyList<CommerceCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await commerceRepository.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .Select(category => new CommerceCategoryDto(
                category.Id,
                category.Name,
                category.Code,
                ToSlug(category.Code),
                category.SortOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CommerceProductDto>> GetProductsAsync(string? search, CancellationToken cancellationToken)
    {
        var products = await QueryProducts(search)
            .OrderBy(product => product.ModelCode)
            .Take(48)
            .ToListAsync(cancellationToken);

        return products.Select(ToProductDto).ToList();
    }

    public async Task<CommerceProductDetailDto?> GetProductAsync(string slug, CancellationToken cancellationToken)
    {
        var products = await QueryProducts(null).ToListAsync(cancellationToken);
        var product = products.FirstOrDefault(item => ToSlug(item.ModelCode).Equals(slug, StringComparison.OrdinalIgnoreCase));

        if (product is null)
        {
            return null;
        }

        var variants = product.Variants
            .OrderBy(variant => variant.Color!.Name)
            .ThenBy(variant => variant.Size!.SortOrder)
            .Select(variant => new CommerceVariantDto(
                variant.Color?.Name ?? string.Empty,
                variant.Size?.Name ?? string.Empty,
                variant.Barcode,
                variant.Stock,
                variant.SalesPrice > 0 ? variant.SalesPrice : variant.PurchasePrice))
            .ToList();

        var dto = ToProductDto(product);
        var seo = await GetSeoAsync($"/product/{slug}", dto.ProductName, $"{dto.ProductName} FIOLIN koleksiyonunda.", cancellationToken);

        return new CommerceProductDetailDto(
            dto.Id,
            dto.ModelCode,
            dto.Slug,
            dto.ProductName,
            dto.Category,
            dto.ImageUrl,
            dto.Price,
            dto.Stock,
            variants,
            seo);
    }

    public async Task<IReadOnlyList<CommerceMenuDto>> GetMenuAsync(CancellationToken cancellationToken)
    {
        var menus = await commerceRepository.Menus
            .AsNoTracking()
            .Where(menu => menu.IsActive)
            .OrderBy(menu => menu.Location)
            .ThenBy(menu => menu.SortOrder)
            .Select(menu => new CommerceMenuDto(menu.Title, menu.Url, menu.Location, menu.SortOrder))
            .ToListAsync(cancellationToken);

        return menus.Count > 0 ? menus : DefaultMenus();
    }

    public async Task<IReadOnlyList<CommerceBannerDto>> GetBannersAsync(CancellationToken cancellationToken)
    {
        var banners = await commerceRepository.Banners
            .AsNoTracking()
            .Where(banner => banner.IsActive)
            .OrderBy(banner => banner.Placement)
            .ThenBy(banner => banner.SortOrder)
            .Select(banner => new CommerceBannerDto(banner.Title, banner.Subtitle, banner.ImageUrl, banner.LinkUrl, banner.Placement))
            .ToListAsync(cancellationToken);

        return banners.Count > 0 ? banners : DefaultBanners();
    }

    public async Task<IReadOnlyList<CommerceSliderDto>> GetSlidersAsync(CancellationToken cancellationToken)
    {
        var sliders = await commerceRepository.Sliders
            .AsNoTracking()
            .Where(slider => slider.IsActive)
            .OrderBy(slider => slider.SortOrder)
            .Select(slider => new CommerceSliderDto(slider.Title, slider.Subtitle, slider.ImageUrl, slider.LinkUrl))
            .ToListAsync(cancellationToken);

        return sliders.Count > 0 ? sliders : DefaultSliders();
    }

    public async Task<CommerceSettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var values = await commerceRepository.Settings
            .AsNoTracking()
            .ToDictionaryAsync(setting => setting.Key, setting => setting.Value, cancellationToken);

        return new CommerceSettingsDto(
            values.GetValueOrDefault("theme.primary", "#111111"),
            values.GetValueOrDefault("theme.secondary", "#b4935a"),
            values.GetValueOrDefault("theme.typography", "Inter, Arial, sans-serif"),
            values.GetValueOrDefault("theme.darkMode", "false").Equals("true", StringComparison.OrdinalIgnoreCase),
            values);
    }

    private IQueryable<Product> QueryProducts(string? search)
    {
        var query = commerceRepository.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Variants)
                .ThenInclude(variant => variant.Color)
            .Include(product => product.Variants)
                .ThenInclude(variant => variant.Size)
            .Where(product => product.Status == "Active");

        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var term = search.Trim().ToLowerInvariant();

        return query.Where(product =>
            product.ModelCode.ToLower().Contains(term) ||
            product.ProductName.ToLower().Contains(term) ||
            product.Variants.Any(variant => variant.Barcode.ToLower().Contains(term)));
    }

    private async Task<CommerceSeoDto> GetSeoAsync(string route, string fallbackTitle, string fallbackDescription, CancellationToken cancellationToken)
    {
        var seo = await commerceRepository.SeoPages.AsNoTracking().FirstOrDefaultAsync(item => item.Route == route, cancellationToken);

        return seo is null
            ? new CommerceSeoDto(fallbackTitle, fallbackDescription, route, fallbackTitle, fallbackDescription, "summary_large_image", null)
            : new CommerceSeoDto(seo.MetaTitle, seo.MetaDescription, seo.Canonical, seo.OpenGraphTitle, seo.OpenGraphDescription, seo.TwitterCard, seo.SchemaJson);
    }

    private static CommerceProductDto ToProductDto(Product product)
    {
        var variants = product.Variants.ToList();
        var price = variants
            .Select(variant => variant.SalesPrice > 0 ? variant.SalesPrice : variant.PurchasePrice)
            .Where(value => value > 0)
            .DefaultIfEmpty(0)
            .Min();

        return new CommerceProductDto(
            product.Id,
            product.ModelCode,
            ToSlug(product.ModelCode),
            product.ProductName,
            product.Category?.Name,
            product.ImageUrl,
            price,
            variants.Sum(variant => variant.Stock));
    }

    private static string ToSlug(string value)
    {
        var normalized = value.ToLower(CultureInfo.GetCultureInfo("tr-TR"))
            .Replace("ı", "i", StringComparison.Ordinal)
            .Replace("ğ", "g", StringComparison.Ordinal)
            .Replace("ü", "u", StringComparison.Ordinal)
            .Replace("ş", "s", StringComparison.Ordinal)
            .Replace("ö", "o", StringComparison.Ordinal)
            .Replace("ç", "c", StringComparison.Ordinal);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private static IReadOnlyList<CommerceMenuDto> DefaultMenus() =>
    [
        new("Yeni Sezon", "/new-season", "header", 1),
        new("Kategoriler", "/categories", "header", 2),
        new("Blog", "/blog", "footer", 1),
        new("İletişim", "/contact", "footer", 2)
    ];

    private static IReadOnlyList<CommerceBannerDto> DefaultBanners() =>
    [
        new("Hafta Sonu Seçkisi", "Zarif ve hızlı kombinler", "https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&w=1400&q=80", "/new-season", "campaign")
    ];

    private static IReadOnlyList<CommerceSliderDto> DefaultSliders() =>
    [
        new("FIOLIN Yeni Sezon", "Modern kadın giyimde premium seçki", "https://images.unsplash.com/photo-1496747611176-843222e1e57c?auto=format&fit=crop&w=1600&q=80", "/new-season")
    ];
}
