namespace FiolinOne.Application.Reports;

public sealed record ReportsQuery(
    string? Search,
    string? Status,
    DateTime? DateFrom,
    DateTime? DateTo);

public sealed record ReportsOverviewDto(
    ReportSummaryDto Summary,
    IReadOnlyList<ProductReportRowDto> Products,
    IReadOnlyList<InventoryReportRowDto> Inventory,
    IReadOnlyList<PurchasingReportRowDto> Purchasing,
    IReadOnlyList<ProductionReportRowDto> Production,
    IReadOnlyList<SalesReportRowDto> Sales,
    IReadOnlyList<StockConsistencyRowDto> StockConsistency);

public sealed record ReportSummaryDto(
    int ProductCount,
    int ProductVariantCount,
    int ProductStockQuantity,
    decimal FabricStockKg,
    decimal PurchaseOrderAmount,
    decimal PurchaseInvoiceAmount,
    int PlannedProductionQuantity,
    int CompletedProductionQuantity,
    decimal SalesAmount,
    int SoldQuantity);

public sealed record ProductReportRowDto(
    Guid Id,
    string ProductCode,
    string ProductName,
    string Brand,
    string Category,
    string Season,
    string Status,
    int VariantCount,
    int StockQuantity);

public sealed record InventoryReportRowDto(
    string Id,
    string InventoryType,
    string Code,
    string Name,
    string Detail,
    decimal Quantity,
    string Unit,
    string Status);

public sealed record PurchasingReportRowDto(
    Guid Id,
    string DocumentNumber,
    string SupplierName,
    DateTime DocumentDate,
    string Status,
    decimal OrderedQuantity,
    decimal ReceivedQuantity,
    decimal RemainingQuantity,
    decimal TotalAmount,
    decimal InvoiceAmount);

public sealed record ProductionReportRowDto(
    Guid Id,
    string ProductionNumber,
    string ProductCode,
    string ProductName,
    DateTime CreatedAt,
    string Reason,
    string Status,
    int PlannedQuantity,
    int WarehouseQuantity);

public sealed record SalesReportRowDto(
    Guid Id,
    string SalesOrderNumber,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    int Quantity,
    decimal TotalAmount);

public sealed record StockConsistencyRowDto(
    string Area,
    decimal IncomingQuantity,
    decimal OutgoingQuantity,
    decimal CurrentStock,
    string Unit,
    string Status);
