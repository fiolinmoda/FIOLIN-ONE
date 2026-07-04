using FiolinOne.Application.Products.Variants;
using FiolinOne.Domain.Products;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Products;

public sealed class ProductVariantRepository(ApplicationDbContext dbContext) : IProductVariantRepository
{
    public Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken)
    {
        return dbContext.Products.AnyAsync(product => product.Id == productId, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await dbContext.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Color)
            .Include(variant => variant.Size)
            .Where(variant => variant.ProductId == productId)
            .OrderBy(variant => variant.Color!.Name)
            .ThenBy(variant => variant.Size!.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<ProductVariant?> GetByIdAsync(Guid productId, Guid variantId, CancellationToken cancellationToken)
    {
        return dbContext.ProductVariants
            .Include(variant => variant.Color)
            .Include(variant => variant.Size)
            .FirstOrDefaultAsync(
                variant => variant.ProductId == productId && variant.Id == variantId,
                cancellationToken);
    }

    public async Task<ProductColor> GetOrCreateColorAsync(string color, CancellationToken cancellationToken)
    {
        var existingColor = await dbContext.ProductColors
            .FirstOrDefaultAsync(item => item.Name.ToLower() == color.ToLower(), cancellationToken);

        if (existingColor is not null)
        {
            return existingColor;
        }

        var productColor = new ProductColor(color);

        await dbContext.ProductColors.AddAsync(productColor, cancellationToken);

        return productColor;
    }

    public async Task<ProductSize> GetOrCreateSizeAsync(string size, CancellationToken cancellationToken)
    {
        var existingSize = await dbContext.ProductSizes
            .FirstOrDefaultAsync(item => item.Name.ToLower() == size.ToLower(), cancellationToken);

        if (existingSize is not null)
        {
            return existingSize;
        }

        var productSize = new ProductSize(size);

        await dbContext.ProductSizes.AddAsync(productSize, cancellationToken);

        return productSize;
    }

    public Task<bool> VariantCombinationExistsAsync(
        Guid productId,
        Guid colorId,
        Guid sizeId,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        return dbContext.ProductVariants.AnyAsync(
            variant =>
                variant.ProductId == productId &&
                variant.ColorId == colorId &&
                variant.SizeId == sizeId &&
                (!excludedId.HasValue || variant.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<bool> BarcodeExistsAsync(string barcode, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.ProductVariants.AnyAsync(
            variant => variant.Barcode == barcode && (!excludedId.HasValue || variant.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<bool> TrendyolSkuExistsAsync(string trendyolSku, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.ProductVariants.AnyAsync(
            variant => variant.TrendyolSku == trendyolSku && (!excludedId.HasValue || variant.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddAsync(ProductVariant variant, CancellationToken cancellationToken)
    {
        await dbContext.ProductVariants.AddAsync(variant, cancellationToken);
    }

    public void Delete(ProductVariant variant)
    {
        dbContext.ProductVariants.Remove(variant);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
