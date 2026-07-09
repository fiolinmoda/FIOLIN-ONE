using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Production;
using FiolinOne.Domain.Production;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Production;

public sealed class ProductionRepository(ApplicationDbContext dbContext) : IProductionRepository
{
    public async Task<PagedResult<ProductionOrder>> GetOrdersAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = IncludeOrderGraph(dbContext.ProductionOrders.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(order =>
                order.ProductionNumber.ToLower().Contains(term) ||
                order.Product!.ProductCode.ToLower().Contains(term) ||
                order.Product.ProductName.ToLower().Contains(term) ||
                order.ProductionReason.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            source = source.Where(order => order.Status == query.Status.Trim());
        }

        source = ApplySorting(source, query.SortBy, query.SortDirection);
        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<ProductionOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return IncludeOrderGraph(dbContext.ProductionOrders)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public Task<bool> ProductionNumberExistsAsync(string productionNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.ProductionOrders.AnyAsync(
            order => order.ProductionNumber == productionNumber && (!excludedId.HasValue || order.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken)
    {
        return dbContext.Products.AnyAsync(product => product.Id == productId, cancellationToken);
    }

    public Task<bool> VariantExistsAsync(Guid variantId, Guid productId, CancellationToken cancellationToken)
    {
        return dbContext.ProductVariants.AnyAsync(variant => variant.Id == variantId && variant.ProductId == productId, cancellationToken);
    }

    public async Task AddOrderAsync(ProductionOrder order, CancellationToken cancellationToken)
    {
        await dbContext.ProductionOrders.AddAsync(order, cancellationToken);
    }

    public async Task ReplaceItemsAsync(Guid productionOrderId, IReadOnlyList<ProductionOrderItem> items, CancellationToken cancellationToken)
    {
        var existing = await dbContext.ProductionOrderItems
            .Where(item => item.ProductionOrderId == productionOrderId)
            .ToListAsync(cancellationToken);

        foreach (var item in existing)
        {
            item.SoftDelete();
        }

        await dbContext.ProductionOrderItems.AddRangeAsync(items, cancellationToken);
    }

    public async Task AddCuttingRecordAsync(CuttingRecord record, CancellationToken cancellationToken)
    {
        await dbContext.CuttingRecords.AddAsync(record, cancellationToken);
    }

    public async Task AddWorkshopShipmentAsync(WorkshopShipment shipment, CancellationToken cancellationToken)
    {
        await dbContext.WorkshopShipments.AddAsync(shipment, cancellationToken);
    }

    public Task<WorkshopShipment?> GetWorkshopShipmentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.WorkshopShipments.FirstOrDefaultAsync(shipment => shipment.Id == id, cancellationToken);
    }

    public Task<int> GetReturnedQuantityForShipmentAsync(Guid workshopShipmentId, CancellationToken cancellationToken)
    {
        return dbContext.WorkshopReturns
            .Where(workshopReturn => workshopReturn.WorkshopShipmentId == workshopShipmentId && !workshopReturn.IsDeleted)
            .SumAsync(workshopReturn => workshopReturn.ReturnedQuantity + workshopReturn.ExtraQuantity, cancellationToken);
    }

    public async Task AddWorkshopReturnAsync(WorkshopReturn workshopReturn, CancellationToken cancellationToken)
    {
        await dbContext.WorkshopReturns.AddAsync(workshopReturn, cancellationToken);
    }

    public async Task AddWarehouseEntryAsync(WarehouseEntry entry, CancellationToken cancellationToken)
    {
        await dbContext.WarehouseEntries.AddAsync(entry, cancellationToken);
    }

    public Task<bool> WarehouseEntryExistsAsync(Guid productionOrderId, CancellationToken cancellationToken)
    {
        return dbContext.WarehouseEntries.AnyAsync(entry => entry.ProductionOrderId == productionOrderId && !entry.IsDeleted, cancellationToken);
    }

    public async Task AddTimelineAsync(ProductionTimelineEntry entry, CancellationToken cancellationToken)
    {
        await dbContext.ProductionTimelineEntries.AddAsync(entry, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionTimelineEntry>> GetTimelineAsync(Guid productionOrderId, CancellationToken cancellationToken)
    {
        return await dbContext.ProductionTimelineEntries
            .AsNoTracking()
            .Where(entry => entry.ProductionOrderId == productionOrderId)
            .OrderBy(entry => entry.EventDate)
            .ThenBy(entry => entry.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var planned = await dbContext.ProductionOrders.CountAsync(order => order.Status == ProductionStatuses.Planned, cancellationToken);
        var cutting = await dbContext.ProductionOrders.CountAsync(order => order.Status == ProductionStatuses.Cutting, cancellationToken);
        var workshop = await dbContext.ProductionOrders.CountAsync(order => order.Status == ProductionStatuses.AtWorkshop, cancellationToken);
        var ironing = await dbContext.ProductionOrders.CountAsync(order => order.Status == ProductionStatuses.AtIroningPackaging, cancellationToken);
        var completed = await dbContext.ProductionOrders.CountAsync(order => order.Status == ProductionStatuses.Completed, cancellationToken);
        return new ProductionDashboardDto(planned, cutting, workshop, ironing, completed);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<ProductionOrder> IncludeOrderGraph(IQueryable<ProductionOrder> source)
    {
        return source
            .Include(order => order.Product)
            .Include(order => order.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant!.Color)
            .Include(order => order.Items)
            .ThenInclude(item => item.ProductVariant)
            .ThenInclude(variant => variant!.Size);
    }

    private static IQueryable<ProductionOrder> ApplySorting(IQueryable<ProductionOrder> source, string? sortBy, string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "product" => descending ? source.OrderByDescending(order => order.Product!.ProductCode) : source.OrderBy(order => order.Product!.ProductCode),
            "quantity" => descending ? source.OrderByDescending(order => order.PlannedQuantity) : source.OrderBy(order => order.PlannedQuantity),
            "status" => descending ? source.OrderByDescending(order => order.Status) : source.OrderBy(order => order.Status),
            _ => descending ? source.OrderByDescending(order => order.ProductionNumber) : source.OrderBy(order => order.ProductionNumber)
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
