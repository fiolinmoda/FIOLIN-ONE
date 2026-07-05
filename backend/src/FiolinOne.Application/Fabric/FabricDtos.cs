using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Fabric;

public sealed record FabricDto(
    Guid Id,
    string FabricCode,
    string FabricName,
    Guid SupplierId,
    string SupplierName,
    Guid ColorId,
    string Color,
    string? Composition,
    decimal Width,
    decimal WeightGsm,
    string Unit,
    decimal PurchasePrice,
    decimal CurrentStockKg,
    decimal MinimumStock,
    decimal ReservedQuantityKg,
    decimal AvailableStockKg,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record FabricMovementDto(
    Guid Id,
    Guid FabricId,
    string FabricCode,
    string FabricName,
    string MovementType,
    decimal QuantityKg,
    decimal UnitPrice,
    Guid? SupplierId,
    string? SupplierName,
    Guid? PurchaseOrderId,
    string? PurchaseNumber,
    string? BatchLot,
    string Warehouse,
    DateTime MovementDate,
    string? Notes,
    DateTime CreatedAt);

public sealed record FabricReservationDto(
    Guid Id,
    Guid FabricId,
    string FabricCode,
    string FabricName,
    string ReservationNumber,
    string ProductionReference,
    decimal ReservedQuantityKg,
    DateTime ReservationDate,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record FabricQuery(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 25)
{
    public QueryParameters ToParameters() => new(Search, Status, SortBy, SortDirection, Page, PageSize);
}
