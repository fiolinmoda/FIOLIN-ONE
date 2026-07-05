namespace FiolinOne.Application.Products;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? search, CancellationToken cancellationToken);
    Task<ProductDto?> GetProductAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);
}
