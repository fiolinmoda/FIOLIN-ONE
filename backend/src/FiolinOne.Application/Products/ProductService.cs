using FiolinOne.Domain.Products;

namespace FiolinOne.Application.Products;

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? search, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(search, cancellationToken);

        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto?> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        return product is null ? null : ToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        await EnsureProductCodeIsUniqueAsync(request.ProductCode, null, cancellationToken);

        var product = new Product(
            request.ProductCode.Trim(),
            request.ProductName.Trim(),
            NormalizeOptional(request.Brand),
            request.Category.Trim(),
            NormalizeOptional(request.Season),
            request.Status.Trim());

        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);

        return ToDto(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        await EnsureProductCodeIsUniqueAsync(request.ProductCode, id, cancellationToken);

        product.Update(
            request.ProductCode.Trim(),
            request.ProductName.Trim(),
            NormalizeOptional(request.Brand),
            request.Category.Trim(),
            NormalizeOptional(request.Season),
            request.Status.Trim());

        await productRepository.SaveChangesAsync(cancellationToken);

        return ToDto(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        productRepository.Delete(product);
        await productRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureProductCodeIsUniqueAsync(
        string productCode,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await productRepository.ExistsByCodeAsync(productCode.Trim(), excludedId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Product code already exists.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static ProductDto ToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.ProductCode,
            product.ProductName,
            product.Brand,
            product.Category,
            product.Season,
            product.Status,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
