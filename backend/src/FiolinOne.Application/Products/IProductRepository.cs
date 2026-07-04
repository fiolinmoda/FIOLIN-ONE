using FiolinOne.Domain.Products;

namespace FiolinOne.Application.Products;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(string? search, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(string productCode, Guid? excludedId, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Delete(Product product);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
