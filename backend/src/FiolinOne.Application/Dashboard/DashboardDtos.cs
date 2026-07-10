namespace FiolinOne.Application.Dashboard;

public sealed record DashboardOverviewDto(
    DashboardMetricDto TodaySales,
    DashboardMetricDto TotalOrders,
    DashboardMetricDto OpenProductionOrders,
    DashboardMetricDto CurrentInventory,
    IReadOnlyList<CriticalProductDto> CriticalProducts,
    IReadOnlyList<RecentDocumentDto> RecentPurchasing,
    IReadOnlyList<RecentDocumentDto> RecentProduction,
    IReadOnlyList<RecentDocumentDto> RecentSales);

public sealed record DashboardMetricDto(
    string Title,
    decimal Value,
    string Unit,
    string Link);

public sealed record CriticalProductDto(
    Guid ProductId,
    Guid VariantId,
    string ProductCode,
    string ProductName,
    string Color,
    string Size,
    int Stock,
    string Link);

public sealed record RecentDocumentDto(
    Guid Id,
    string DocumentNumber,
    string Title,
    DateTime Date,
    string Status,
    decimal Quantity,
    decimal Amount,
    string Link);
