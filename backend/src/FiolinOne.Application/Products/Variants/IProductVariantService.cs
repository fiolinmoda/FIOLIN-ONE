namespace FiolinOne.Application.Products.Variants;

public interface IProductVariantService
{
    Task<IReadOnlyList<ProductVariantDto>> GetVariantsAsync(Guid productId, CancellationToken cancellationToken);
    Task<ProductVariantDto?> GetVariantAsync(Guid productId, Guid variantId, CancellationToken cancellationToken);
    Task<ProductVariantDto?> CreateVariantAsync(Guid productId, CreateProductVariantRequest request, CancellationToken cancellationToken);
    Task<ProductVariantDto?> UpdateVariantAsync(Guid productId, Guid variantId, UpdateProductVariantRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteVariantAsync(Guid productId, Guid variantId, CancellationToken cancellationToken);
}
