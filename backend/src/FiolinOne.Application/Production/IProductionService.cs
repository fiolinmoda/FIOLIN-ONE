using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Production;

public interface IProductionService
{
    Task<ProductionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
    Task<PagedResult<ProductionOrderDto>> GetOrdersAsync(ProductionQuery query, CancellationToken cancellationToken);
    Task<ProductionOrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductionOrderDto> CreateOrderAsync(CreateProductionOrderRequest request, CancellationToken cancellationToken);
    Task<ProductionOrderDto?> UpdateOrderAsync(Guid id, UpdateProductionOrderRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteOrderAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductionOrderDto?> UpdateStatusAsync(Guid id, UpdateProductionStatusRequest request, CancellationToken cancellationToken);

    Task<CuttingRecordDto> CreateCuttingAsync(CreateCuttingRecordRequest request, CancellationToken cancellationToken);
    Task<WorkshopShipmentDto> CreateWorkshopShipmentAsync(CreateWorkshopShipmentRequest request, CancellationToken cancellationToken);
    Task<WorkshopReturnDto> CreateWorkshopReturnAsync(CreateWorkshopReturnRequest request, CancellationToken cancellationToken);
    Task<ProductionOrderDto?> SendToIroningPackagingAsync(Guid id, CancellationToken cancellationToken);
    Task<WarehouseEntryDto> CreateWarehouseEntryAsync(CreateWarehouseEntryRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductionTimelineDto>> GetTimelineAsync(Guid productionOrderId, CancellationToken cancellationToken);
}
