using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Production;

public sealed record ProductionOrderDto(
    Guid Id,
    string ProductionNumber,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    int PlannedQuantity,
    string ProductionReason,
    string? Notes,
    string Status,
    IReadOnlyList<ProductionOrderItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ProductionOrderItemDto(
    Guid Id,
    Guid ProductVariantId,
    string VariantName,
    int PlannedQuantity,
    bool BarcodeGenerated,
    bool BarcodePrinted,
    string? BarcodeValue);

public sealed record CuttingRecordDto(Guid Id, Guid ProductionOrderId, Guid FabricId, string FabricName, decimal ConsumedWeightKg, decimal WasteWeightKg, DateTime CuttingDate, string? OperatorName, string? Notes);
public sealed record WorkshopShipmentDto(Guid Id, Guid ProductionOrderId, string Workshop, DateTime ShipmentDate, DateTime? ExpectedReturnDate, int SentQuantity, string? Notes, string Status);
public sealed record WorkshopReturnDto(Guid Id, Guid ProductionOrderId, Guid? WorkshopShipmentId, int ReturnedQuantity, int ExtraQuantity, int MissingQuantity, DateTime ReturnDate, string? Notes);
public sealed record WarehouseEntryDto(Guid Id, Guid ProductionOrderId, int ActualQuantity, DateTime WarehouseDate, string? Notes);
public sealed record ProductionTimelineDto(Guid Id, Guid ProductionOrderId, string EventType, string Description, DateTime EventDate, DateTime CreatedAt);
public sealed record ProductionDashboardDto(int ProductionPlanned, int InCutting, int AtWorkshop, int IroningPackaging, int Completed);

public sealed record ProductionQuery(string? Search, string? Status, string? SortBy, string? SortDirection, int Page = 1, int PageSize = 25)
{
    public QueryParameters ToParameters() => new(Search, Status, SortBy, SortDirection, Page, PageSize);
}
