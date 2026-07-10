using FiolinOne.Application.Products;
using FiolinOne.Domain.Products;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Products;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        IQueryable<Product> query = dbContext.Products
            .AsNoTracking()
            .Include(product => product.Brand)
            .Include(product => product.Category)
            .Include(product => product.Season)
            .Include(product => product.Variants)
                .ThenInclude(variant => variant.Color)
            .Include(product => product.Variants)
                .ThenInclude(variant => variant.Size);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(product =>
                product.ProductCode.ToLower().Contains(term) ||
                product.ProductName.ToLower().Contains(term) ||
                (product.Brand != null && product.Brand.Name.ToLower().Contains(term)) ||
                (product.Category != null && product.Category.Name.ToLower().Contains(term)) ||
                (product.Season != null && product.Season.Name.ToLower().Contains(term)) ||
                product.Variants.Any(variant =>
                    (variant.Color != null && variant.Color.Name.ToLower().Contains(term)) ||
                    (variant.Size != null && variant.Size.Name.ToLower().Contains(term))) ||
                product.Status.ToLower().Contains(term));
        }

        return await query
            .OrderBy(product => product.ProductCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductListRowDto>> GetListRowsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.ProductCode)
            .SelectMany(
                product => product.Variants.DefaultIfEmpty(),
                (product, variant) => new ProductListRowDto(
                    product.Id,
                    product.ProductCode,
                    product.ProductName,
                    product.BrandId,
                    product.Brand == null ? null : product.Brand.Name,
                    product.CategoryId,
                    product.Category == null ? null : product.Category.Name,
                    product.SeasonId,
                    product.Season == null ? null : product.Season.Name,
                    product.Status,
                    product.ImageUrl,
                    product.CreatedAt,
                    product.UpdatedAt,
                    variant == null ? null : variant.Id,
                    variant == null ? null : variant.ColorId,
                    variant == null || variant.Color == null ? null : variant.Color.Name,
                    variant == null ? null : variant.SizeId,
                    variant == null || variant.Size == null ? null : variant.Size.Name,
                    variant == null || variant.Size == null ? null : variant.Size.SortOrder,
                    variant == null ? null : variant.Barcode,
                    variant == null ? 0 : variant.Stock,
                    variant == null ? 0 : variant.PurchasePrice,
                    variant == null ? 0 : variant.SalesPrice))
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .Include(product => product.Brand)
            .Include(product => product.Category)
            .Include(product => product.Season)
            .Include(product => product.Variants)
                .ThenInclude(variant => variant.Color)
            .Include(product => product.Variants)
                .ThenInclude(variant => variant.Size)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
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
