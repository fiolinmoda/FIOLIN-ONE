using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Sales;
using FiolinOne.Domain.Products;
using FiolinOne.Domain.Sales;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Sales;

public sealed class SalesRepository(ApplicationDbContext dbContext) : ISalesRepository
{
    public async Task<PagedResult<SalesOrder>> GetOrdersAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = IncludeOrderGraph(dbContext.SalesOrders.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = $"%{query.Search.Trim()}%";
            source = source.Where(order =>
                EF.Functions.ILike(order.SalesOrderNumber, term) ||
                EF.Functions.ILike(order.CustomerName, term) ||
                order.Items.Any(item =>
                    EF.Functions.ILike(item.ProductVariant!.Barcode, term) ||
                    EF.Functions.ILike(item.ProductVariant.Product!.ProductCode, term) ||
                    EF.Functions.ILike(item.ProductVariant.Product.ProductName, term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            source = source.Where(order => order.Status == query.Status.Trim());
        }

        source = ApplySorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<SalesOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return IncludeOrderGraph(dbContext.SalesOrders)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public Task<bool> SalesOrderNumberExistsAsync(string salesOrderNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.SalesOrders.AnyAsync(
            order => order.SalesOrderNumber == salesOrderNumber && (!excludedId.HasValue || order.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<ProductVariant?> GetVariantByIdAsync(Guid productVariantId, CancellationToken cancellationToken)
    {
        return dbContext.ProductVariants
            .Include(variant => variant.Product)
            .Include(variant => variant.Color)
            .Include(variant => variant.Size)
            .FirstOrDefaultAsync(variant => variant.Id == productVariantId, cancellationToken);
    }

    public async Task AddOrderAsync(SalesOrder order, CancellationToken cancellationToken)
    {
        await dbContext.SalesOrders.AddAsync(order, cancellationToken);
    }

    public async Task ReplaceItemsAsync(SalesOrder order, IReadOnlyList<SalesOrderItem> items, CancellationToken cancellationToken)
    {
        var existingItems = await dbContext.SalesOrderItems
            .Where(item => item.SalesOrderId == order.Id)
            .ToListAsync(cancellationToken);

        foreach (var item in existingItems)
        {
            item.SoftDelete();
        }

        await dbContext.SalesOrderItems.AddRangeAsync(items, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<SalesOrder> IncludeOrderGraph(IQueryable<SalesOrder> source)
    {
        return source
            .Include(order => order.Items.Where(item => !item.IsDeleted))
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Product)
            .Include(order => order.Items.Where(item => !item.IsDeleted))
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Color)
            .Include(order => order.Items.Where(item => !item.IsDeleted))
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Size);
    }

    private static IQueryable<SalesOrder> ApplySorting(IQueryable<SalesOrder> source, string? sortBy, string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.Trim().Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant() switch
        {
            "customer" => descending ? source.OrderByDescending(order => order.CustomerName) : source.OrderBy(order => order.CustomerName),
            "orderdate" => descending ? source.OrderByDescending(order => order.OrderDate) : source.OrderBy(order => order.OrderDate),
            "totalamount" => descending ? source.OrderByDescending(order => order.TotalAmount) : source.OrderBy(order => order.TotalAmount),
            "status" => descending ? source.OrderByDescending(order => order.Status) : source.OrderBy(order => order.Status),
            _ => descending ? source.OrderByDescending(order => order.SalesOrderNumber) : source.OrderBy(order => order.SalesOrderNumber)
        };
    }

    private static async Task<PagedResult<T>> ToPagedResultAsync<T>(IQueryable<T> source, QueryParameters query, CancellationToken cancellationToken)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var totalItems = await source.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<T>(items, page, pageSize, totalItems, totalPages);
    }
}
