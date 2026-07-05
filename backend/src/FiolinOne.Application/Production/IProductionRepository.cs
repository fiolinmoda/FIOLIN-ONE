using FiolinOne.Application.Common.Models;
using FiolinOne.Domain.Production;

namespace FiolinOne.Application.Production;

public interface IProductionRepository
{
    Task<PagedResult<ProductionOrder>> GetOrdersAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<ProductionOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ProductionNumberExistsAsync(string productionNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken);
    Task<bool> VariantExistsAsync(Guid variantId, Guid productId, CancellationToken cancellationToken);
    Task AddOrderAsync(ProductionOrder order, CancellationToken cancellationToken);
    Task ReplaceItemsAsync(Guid productionOrderId, IReadOnlyList<ProductionOrderItem> items, CancellationToken cancellationToken);

    Task AddCuttingRecordAsync(CuttingRecord record, CancellationToken cancellationToken);
    Task AddWorkshopShipmentAsync(WorkshopShipment shipment, CancellationToken cancellationToken);
    Task AddWorkshopReturnAsync(WorkshopReturn workshopReturn, CancellationToken cancellationToken);
    Task AddWarehouseEntryAsync(WarehouseEntry entry, CancellationToken cancellationToken);
    Task AddTimelineAsync(ProductionTimelineEntry entry, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductionTimelineEntry>> GetTimelineAsync(Guid productionOrderId, CancellationToken cancellationToken);
    Task<ProductionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
