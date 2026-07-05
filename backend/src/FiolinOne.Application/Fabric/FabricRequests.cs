namespace FiolinOne.Application.Fabric;

public sealed record CreateFabricRequest(
    string FabricCode,
    string FabricName,
    Guid SupplierId,
    Guid ColorId,
    string? Composition,
    decimal Width,
    decimal WeightGsm,
    string Unit,
    decimal PurchasePrice,
    decimal CurrentStockKg,
    decimal MinimumStock,
    string Status,
    string? Notes);

public sealed record UpdateFabricRequest(
    string FabricCode,
    string FabricName,
    Guid SupplierId,
    Guid ColorId,
    string? Composition,
    decimal Width,
    decimal WeightGsm,
    string Unit,
    decimal PurchasePrice,
    decimal MinimumStock,
    string Status,
    string? Notes);

public sealed record CreateFabricPurchaseMovementRequest(
    Guid SupplierId,
    Guid? PurchaseOrderId,
    Guid FabricId,
    Guid ColorId,
    string? BatchLot,
    decimal TotalWeightKg,
    decimal UnitPrice,
    string Warehouse,
    DateTime ArrivalDate,
    string? Notes);

public sealed record CreateFabricMovementRequest(
    Guid FabricId,
    string MovementType,
    decimal QuantityKg,
    decimal UnitPrice,
    Guid? SupplierId,
    Guid? PurchaseOrderId,
    string? BatchLot,
    string Warehouse,
    DateTime MovementDate,
    string? Notes);

public sealed record CreateFabricConsumptionRequest(
    Guid FabricId,
    decimal QuantityKg,
    string ProductionReference,
    DateTime ConsumptionDate,
    string? Notes);

public sealed record CreateFabricReservationRequest(
    Guid FabricId,
    string ReservationNumber,
    string ProductionReference,
    decimal ReservedQuantityKg,
    DateTime ReservationDate,
    string Status,
    string? Notes);

public sealed record UpdateFabricReservationRequest(
    Guid FabricId,
    string ReservationNumber,
    string ProductionReference,
    decimal ReservedQuantityKg,
    DateTime ReservationDate,
    string Status,
    string? Notes);
