using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.Production;
using FiolinOne.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.Dashboard;

public sealed class DashboardService(IApplicationDbContext dbContext) : IDashboardService
{
    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var todaySalesAmount = await dbContext.SalesOrders
            .AsNoTracking()
            .Where(order => order.Status == SalesOrderStatuses.Completed && order.OrderDate >= today && order.OrderDate < tomorrow)
            .SumAsync(order => order.TotalAmount, cancellationToken);

        var totalOrders = await dbContext.SalesOrders
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var openProductionOrders = await dbContext.ProductionOrders
            .AsNoTracking()
            .CountAsync(order => order.Status != ProductionStatuses.Completed && order.Status != ProductionStatuses.Cancelled, cancellationToken);

        var productStock = await dbContext.ProductVariants
            .AsNoTracking()
            .SumAsync(variant => variant.Stock, cancellationToken);

        var fabricStock = await dbContext.Fabrics
            .AsNoTracking()
            .SumAsync(fabric => fabric.CurrentStockKg, cancellationToken);

        var criticalProducts = await dbContext.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Product)
            .Include(variant => variant.Color)
            .Include(variant => variant.Size)
            .Where(variant => variant.Stock <= 5)
            .OrderBy(variant => variant.Stock)
            .ThenBy(variant => variant.Product!.ProductCode)
            .Take(6)
            .Select(variant => new CriticalProductDto(
                variant.ProductId,
                variant.Id,
                variant.Product!.ProductCode,
                variant.Product.ProductName,
                variant.Color!.Name,
                variant.Size!.Name,
                variant.Stock,
                $"/products/{variant.ProductId}"))
            .ToListAsync(cancellationToken);

        var recentPurchasing = await dbContext.PurchaseOrders
            .AsNoTracking()
            .Include(order => order.Supplier)
            .Include(order => order.Items)
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.CreatedAtUtc)
            .Take(5)
            .Select(order => new RecentDocumentDto(
                order.Id,
                order.PurchaseNumber,
                order.Supplier!.SupplierName,
                order.OrderDate,
                order.Status,
                order.Items.Sum(item => item.Quantity),
                order.Items.Sum(item => item.Quantity * item.UnitPrice),
                $"/purchasing/orders/{order.Id}"))
            .ToListAsync(cancellationToken);

        var recentProduction = await dbContext.ProductionOrders
            .AsNoTracking()
            .Include(order => order.Product)
            .OrderByDescending(order => order.CreatedAtUtc)
            .Take(5)
            .Select(order => new RecentDocumentDto(
                order.Id,
                order.ProductionNumber,
                order.Product!.ProductName,
                order.CreatedAtUtc,
                order.Status,
                order.PlannedQuantity,
                0,
                $"/production/orders/{order.Id}"))
            .ToListAsync(cancellationToken);

        var recentSales = await dbContext.SalesOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.CreatedAtUtc)
            .Take(5)
            .Select(order => new RecentDocumentDto(
                order.Id,
                order.SalesOrderNumber,
                order.CustomerName,
                order.OrderDate,
                order.Status,
                order.Items.Sum(item => item.Quantity),
                order.TotalAmount,
                $"/sales/orders/{order.Id}"))
            .ToListAsync(cancellationToken);

        return new DashboardOverviewDto(
            new DashboardMetricDto("Bugünkü Satış", todaySalesAmount, "TL", "/sales"),
            new DashboardMetricDto("Toplam Sipariş", totalOrders, "Adet", "/sales"),
            new DashboardMetricDto("Açık Üretim Emri", openProductionOrders, "Adet", "/production/orders"),
            new DashboardMetricDto("Mevcut Stok", productStock + fabricStock, "Adet/Kg", "/warehouse/product-stock"),
            criticalProducts,
            recentPurchasing,
            recentProduction,
            recentSales);
    }
}
