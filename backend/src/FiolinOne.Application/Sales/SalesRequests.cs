namespace FiolinOne.Application.Sales;

public sealed record CreateSalesOrderRequest(
    string SalesOrderNumber,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    string? Notes,
    IReadOnlyList<SalesOrderItemRequest> Items);

public sealed record UpdateSalesOrderRequest(
    string SalesOrderNumber,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    string? Notes,
    IReadOnlyList<SalesOrderItemRequest> Items);

public sealed record SalesOrderItemRequest(
    Guid? Id,
    Guid ProductVariantId,
    int Quantity,
    decimal UnitPrice);
