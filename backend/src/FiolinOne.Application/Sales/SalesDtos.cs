namespace FiolinOne.Application.Sales;

public sealed record SalesOrderDto(
    Guid Id,
    string SalesOrderNumber,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    string? Notes,
    IReadOnlyList<SalesOrderItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record SalesOrderItemDto(
    Guid Id,
    Guid ProductVariantId,
    string ProductCode,
    string ProductName,
    string Color,
    string Size,
    string Barcode,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    int AvailableStock);
