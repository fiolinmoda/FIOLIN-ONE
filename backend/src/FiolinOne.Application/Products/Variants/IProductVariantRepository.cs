using FiolinOne.Domain.Products;

namespace FiolinOne.Application.Products.Variants;

public interface IProductVariantRepository
{
    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<ProductVariant?> GetByIdAsync(Guid productId, Guid variantId, CancellationToken cancellationToken);
    Task<bool> VariantCombinationExistsAsync(Guid productId, Guid colorId, Guid sizeId, Guid? excludedId, CancellationToken cancellationToken);
    Task<bool> BarcodeExistsAsync(string barcode, Guid? excludedId, CancellationToken cancellationToken);
    Task<bool> TrendyolSkuExistsAsync(string trendyolSku, Guid? excludedId, CancellationToken cancellationToken);
    Task AddAsync(ProductVariant variant, CancellationToken cancellationToken);
    void Delete(ProductVariant variant);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
