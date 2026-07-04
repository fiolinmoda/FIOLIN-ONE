using FiolinOne.Application.Products;
using FiolinOne.Domain.Products;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Products;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(product =>
                product.ProductCode.ToLower().Contains(term) ||
                product.ProductName.ToLower().Contains(term) ||
                (product.Brand != null && product.Brand.ToLower().Contains(term)) ||
                product.Category.ToLower().Contains(term) ||
                (product.Season != null && product.Season.ToLower().Contains(term)) ||
                product.Status.ToLower().Contains(term));
        }

        return await query
            .OrderBy(product => product.ProductCode)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string productCode, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.Products.AnyAsync(
            product => product.ProductCode == productCode && (!excludedId.HasValue || product.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public void Delete(Product product)
    {
        dbContext.Products.Remove(product);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
