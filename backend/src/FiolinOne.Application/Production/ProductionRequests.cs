namespace FiolinOne.Application.Production;

public sealed record CreateProductionOrderRequest(
    string ProductionNumber,
    Guid ProductId,
    int PlannedQuantity,
    string ProductionReason,
    string? Notes,
    string Status,
    IReadOnlyList<ProductionOrderItemRequest> Items);

public sealed record UpdateProductionOrderRequest(
    string ProductionNumber,
    Guid ProductId,
    int PlannedQuantity,
    string ProductionReason,
    string? Notes,
    string Status,
    IReadOnlyList<ProductionOrderItemRequest> Items);

public sealed record ProductionOrderItemRequest(Guid ProductVariantId, int PlannedQuantity);

public sealed record CreateCuttingRecordRequest(Guid ProductionOrderId, Guid FabricId, decimal ConsumedWeightKg, decimal WasteWeightKg, DateTime CuttingDate, string? OperatorName, string? Notes);
public sealed record CreateWorkshopShipmentRequest(Guid ProductionOrderId, string Workshop, DateTime ShipmentDate, DateTime? ExpectedReturnDate, int SentQuantity, string? Notes, string Status);
public sealed record CreateWorkshopReturnRequest(Guid ProductionOrderId, Guid? WorkshopShipmentId, int ReturnedQuantity, int ExtraQuantity, int MissingQuantity, DateTime ReturnDate, string? Notes);
public sealed record CreateWarehouseEntryRequest(Guid ProductionOrderId, int ActualQuantity, DateTime WarehouseDate, string? Notes);
public sealed record UpdateProductionStatusRequest(string Status);
