using FiolinOne.Application.MasterData;
using FiolinOne.Domain.Products;

namespace FiolinOne.Application.Products.Variants;

public sealed class ProductVariantService(
    IProductVariantRepository productVariantRepository,
    IMasterDataRepository masterDataRepository) : IProductVariantService
{
    public async Task<IReadOnlyList<ProductVariantDto>> GetVariantsAsync(Guid productId, CancellationToken cancellationToken)
    {
        var variants = await productVariantRepository.GetByProductIdAsync(productId, cancellationToken);

        return variants.Select(ToDto).ToList();
    }

    public async Task<ProductVariantDto?> GetVariantAsync(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        var variant = await productVariantRepository.GetByIdAsync(productId, variantId, cancellationToken);

        return variant is null ? null : ToDto(variant);
    }

    public async Task<ProductVariantDto?> CreateVariantAsync(
        Guid productId,
        CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        if (!await productVariantRepository.ProductExistsAsync(productId, cancellationToken))
        {
            return null;
        }

        await EnsureMasterDataExistsAsync(request.ColorId, "colors", cancellationToken);
        await EnsureMasterDataExistsAsync(request.SizeId, "sizes", cancellationToken);

        await EnsureUniqueAsync(
            productId,
            request.ColorId,
            request.SizeId,
            request.Barcode,
            request.TrendyolSku,
            null,
            cancellationToken);

        var variant = new ProductVariant(
            productId,
            request.ColorId,
            request.SizeId,
            request.Barcode.Trim(),
            NormalizeOptional(request.TrendyolSku),
            request.Stock,
            request.Status.Trim());

        await productVariantRepository.AddAsync(variant, cancellationToken);
        await productVariantRepository.SaveChangesAsync(cancellationToken);

        variant = await productVariantRepository.GetByIdAsync(productId, variant.Id, cancellationToken) ?? variant;

        return ToDto(variant);
    }

    public async Task<ProductVariantDto?> UpdateVariantAsync(
        Guid productId,
        Guid variantId,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var variant = await productVariantRepository.GetByIdAsync(productId, variantId, cancellationToken);

        if (variant is null)
        {
            return null;
        }

        await EnsureMasterDataExistsAsync(request.ColorId, "colors", cancellationToken);
        await EnsureMasterDataExistsAsync(request.SizeId, "sizes", cancellationToken);

        await EnsureUniqueAsync(
            productId,
            request.ColorId,
            request.SizeId,
            request.Barcode,
            request.TrendyolSku,
            variantId,
            cancellationToken);

        variant.Update(
            request.ColorId,
            request.SizeId,
            request.Barcode.Trim(),
            NormalizeOptional(request.TrendyolSku),
            request.Stock,
            request.Status.Trim());

        await productVariantRepository.SaveChangesAsync(cancellationToken);

        variant = await productVariantRepository.GetByIdAsync(productId, variantId, cancellationToken) ?? variant;

        return ToDto(variant);
    }

    public async Task<bool> DeleteVariantAsync(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        var variant = await productVariantRepository.GetByIdAsync(productId, variantId, cancellationToken);

        if (variant is null)
        {
            return false;
        }

        productVariantRepository.Delete(variant);
        await productVariantRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureUniqueAsync(
        Guid productId,
        Guid colorId,
        Guid sizeId,
        string barcode,
        string? trendyolSku,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        if (await productVariantRepository.VariantCombinationExistsAsync(productId, colorId, sizeId, excludedId, cancellationToken))
        {
            throw new InvalidOperationException("A variant with the same product, color, and size already exists.");
        }

        if (await productVariantRepository.BarcodeExistsAsync(barcode.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Barcode already exists.");
        }

        var normalizedTrendyolSku = NormalizeOptional(trendyolSku);

        if (normalizedTrendyolSku is not null &&
            await productVariantRepository.TrendyolSkuExistsAsync(normalizedTrendyolSku, excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Trendyol SKU already exists.");
        }
    }

    private async Task EnsureMasterDataExistsAsync(Guid id, string type, CancellationToken cancellationToken)
    {
        if (!await masterDataRepository.ExistsAsync(type, id, cancellationToken))
        {
            throw new InvalidOperationException("Selected master data item does not exist.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static ProductVariantDto ToDto(ProductVariant variant)
    {
        return new ProductVariantDto(
            variant.Id,
            variant.ProductId,
            variant.ColorId,
            variant.Color?.Name ?? string.Empty,
            variant.SizeId,
            variant.Size?.Name ?? string.Empty,
            variant.Barcode,
            variant.TrendyolSku,
            variant.Stock,
            variant.Status,
            variant.CreatedAt,
            variant.UpdatedAt);
    }
}
