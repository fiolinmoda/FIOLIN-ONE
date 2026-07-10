using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.Fabric;
using FiolinOne.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.Reports;

public sealed class ReportsService(IApplicationDbContext dbContext) : IReportsService
{
    public async Task<ReportsOverviewDto> GetOverviewAsync(ReportsQuery query, CancellationToken cancellationToken)
    {
        var search = query.Search?.Trim();
        var status = query.Status?.Trim();
        var dateFrom = query.DateFrom.HasValue ? DateTime.SpecifyKind(query.DateFrom.Value.Date, DateTimeKind.Utc) : (DateTime?)null;
        var dateToExclusive = query.DateTo.HasValue ? DateTime.SpecifyKind(query.DateTo.Value.Date.AddDays(1), DateTimeKind.Utc) : (DateTime?)null;

        var products = await dbContext.Products
            .AsNoTracking()
            .Include(product => product.Brand)
            .Include(product => product.Category)
            .Include(product => product.Season)
            .ToListAsync(cancellationToken);

        var variants = await dbContext.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Product)
            .Include(variant => variant.Color)
            .Include(variant => variant.Size)
            .ToListAsync(cancellationToken);

        var fabrics = await dbContext.Fabrics
            .AsNoTracking()
            .Include(fabric => fabric.Supplier)
            .Include(fabric => fabric.Color)
            .ToListAsync(cancellationToken);

        var fabricMovements = await dbContext.FabricMovements
            .AsNoTracking()
            .Where(movement => (!dateFrom.HasValue || movement.MovementDate >= dateFrom.Value) && (!dateToExclusive.HasValue || movement.MovementDate < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var purchaseOrders = await dbContext.PurchaseOrders
            .AsNoTracking()
            .Include(order => order.Supplier)
            .Include(order => order.Items)
            .Where(order => (!dateFrom.HasValue || order.OrderDate >= dateFrom.Value) && (!dateToExclusive.HasValue || order.OrderDate < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var goodsReceipts = await dbContext.GoodsReceipts
            .AsNoTracking()
            .Include(receipt => receipt.Items)
            .Where(receipt => (!dateFrom.HasValue || receipt.ReceiptDate >= dateFrom.Value) && (!dateToExclusive.HasValue || receipt.ReceiptDate < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var purchaseInvoices = await dbContext.PurchaseInvoices
            .AsNoTracking()
            .Where(invoice => (!dateFrom.HasValue || invoice.InvoiceDate >= dateFrom.Value) && (!dateToExclusive.HasValue || invoice.InvoiceDate < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var productionOrders = await dbContext.ProductionOrders
            .AsNoTracking()
            .Include(order => order.Product)
            .Include(order => order.Items)
            .Where(order => (!dateFrom.HasValue || order.CreatedAtUtc >= dateFrom.Value) && (!dateToExclusive.HasValue || order.CreatedAtUtc < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var warehouseEntries = await dbContext.WarehouseEntries
            .AsNoTracking()
            .Where(entry => (!dateFrom.HasValue || entry.WarehouseDate >= dateFrom.Value) && (!dateToExclusive.HasValue || entry.WarehouseDate < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var salesOrders = await dbContext.SalesOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => (!dateFrom.HasValue || order.OrderDate >= dateFrom.Value) && (!dateToExclusive.HasValue || order.OrderDate < dateToExclusive.Value))
            .ToListAsync(cancellationToken);

        var productRows = BuildProductRows(products, variants)
            .Where(row => MatchesStatus(row.Status, status))
            .Where(row => MatchesSearch(search, row.ProductCode, row.ProductName, row.Brand, row.Category, row.Season))
            .OrderBy(row => row.ProductCode)
            .ToList();

        var inventoryRows = BuildInventoryRows(variants, fabrics)
            .Where(row => MatchesStatus(row.Status, status))
            .Where(row => MatchesSearch(search, row.Code, row.Name, row.Detail, row.InventoryType))
            .OrderBy(row => row.InventoryType)
            .ThenBy(row => row.Code)
            .ToList();

        var invoiceAmountsByOrderId = purchaseInvoices
            .Where(invoice => invoice.PurchaseOrderId.HasValue)
            .GroupBy(invoice => invoice.PurchaseOrderId!.Value)
            .ToDictionary(group => group.Key, group => group.Sum(invoice => invoice.InvoiceAmount));

        var receivedQuantitiesByOrderId = goodsReceipts
            .Where(receipt => receipt.PurchaseOrderId.HasValue)
            .GroupBy(receipt => receipt.PurchaseOrderId!.Value)
            .ToDictionary(group => group.Key, group => group.Sum(receipt => receipt.Items.Sum(item => item.ReceivedQuantity)));

        var purchasingRows = purchaseOrders
            .Select(order =>
            {
                var orderedQuantity = order.Items.Sum(item => item.Quantity);
                var receivedQuantity = receivedQuantitiesByOrderId.GetValueOrDefault(order.Id);
                var totalAmount = order.Items.Sum(item => item.Quantity * item.UnitPrice);

                return new PurchasingReportRowDto(
                    order.Id,
                    order.PurchaseNumber,
                    order.Supplier?.SupplierName ?? string.Empty,
                    order.OrderDate,
                    order.Status,
                    orderedQuantity,
                    receivedQuantity,
                    orderedQuantity - receivedQuantity,
                    totalAmount,
                    invoiceAmountsByOrderId.GetValueOrDefault(order.Id));
            })
            .Where(row => MatchesStatus(row.Status, status))
            .Where(row => MatchesSearch(search, row.DocumentNumber, row.SupplierName, row.Status))
            .OrderByDescending(row => row.DocumentDate)
            .ThenBy(row => row.DocumentNumber)
            .ToList();

        var warehouseQuantityByOrderId = warehouseEntries
            .GroupBy(entry => entry.ProductionOrderId)
            .ToDictionary(group => group.Key, group => group.Sum(entry => entry.ActualQuantity));

        var productionRows = productionOrders
            .Select(order => new ProductionReportRowDto(
                order.Id,
                order.ProductionNumber,
                order.Product?.ProductCode ?? string.Empty,
                order.Product?.ProductName ?? string.Empty,
                order.CreatedAtUtc,
                order.ProductionReason,
                order.Status,
                order.PlannedQuantity,
                warehouseQuantityByOrderId.GetValueOrDefault(order.Id)))
            .Where(row => MatchesStatus(row.Status, status))
            .Where(row => MatchesSearch(search, row.ProductionNumber, row.ProductCode, row.ProductName, row.Reason, row.Status))
            .OrderByDescending(row => row.CreatedAt)
            .ThenBy(row => row.ProductionNumber)
            .ToList();

        var salesRows = salesOrders
            .Select(order => new SalesReportRowDto(
                order.Id,
                order.SalesOrderNumber,
                order.CustomerName,
                order.OrderDate,
                order.Status,
                order.Items.Sum(item => item.Quantity),
                order.TotalAmount))
            .Where(row => MatchesStatus(row.Status, status))
            .Where(row => MatchesSearch(search, row.SalesOrderNumber, row.CustomerName, row.Status))
            .OrderByDescending(row => row.OrderDate)
            .ThenBy(row => row.SalesOrderNumber)
            .ToList();

        var summary = new ReportSummaryDto(
            productRows.Count,
            variants.Count,
            variants.Sum(variant => variant.Stock),
            fabrics.Sum(fabric => fabric.CurrentStockKg),
            purchasingRows.Sum(row => row.TotalAmount),
            purchaseInvoices.Sum(invoice => invoice.InvoiceAmount),
            productionRows.Sum(row => row.PlannedQuantity),
            productionRows.Where(row => row.Status == "COMPLETED").Sum(row => row.WarehouseQuantity),
            salesRows.Sum(row => row.TotalAmount),
            salesRows.Where(row => row.Status == SalesOrderStatuses.Completed).Sum(row => row.Quantity));

        var stockConsistencyRows = BuildStockConsistencyRows(fabricMovements, fabrics, variants, salesOrders, warehouseEntries);

        return new ReportsOverviewDto(summary, productRows, inventoryRows, purchasingRows, productionRows, salesRows, stockConsistencyRows);
    }

    private static IEnumerable<ProductReportRowDto> BuildProductRows(IEnumerable<Domain.Products.Product> products, IEnumerable<Domain.Products.ProductVariant> variants)
    {
        var variantGroups = variants.GroupBy(variant => variant.ProductId).ToDictionary(group => group.Key, group => group.ToList());

        foreach (var product in products)
        {
            var productVariants = variantGroups.GetValueOrDefault(product.Id) ?? [];

            yield return new ProductReportRowDto(
                product.Id,
                product.ProductCode,
                product.ProductName,
                product.Brand?.Name ?? string.Empty,
                product.Category?.Name ?? string.Empty,
                product.Season?.Name ?? string.Empty,
                product.Status,
                productVariants.Count,
                productVariants.Sum(variant => variant.Stock));
        }
    }

    private static IEnumerable<InventoryReportRowDto> BuildInventoryRows(IEnumerable<Domain.Products.ProductVariant> variants, IEnumerable<Domain.Fabric.Fabric> fabrics)
    {
        foreach (var variant in variants)
        {
            yield return new InventoryReportRowDto(
                $"variant-{variant.Id}",
                "Ürün Stoğu",
                variant.Product?.ProductCode ?? string.Empty,
                variant.Product?.ProductName ?? string.Empty,
                $"{variant.Color?.Name ?? "-"} / {variant.Size?.Name ?? "-"} / {variant.Barcode}",
                variant.Stock,
                "Adet",
                variant.Status);
        }

        foreach (var fabric in fabrics)
        {
            yield return new InventoryReportRowDto(
                $"fabric-{fabric.Id}",
                "Kumaş Stoğu",
                fabric.FabricCode,
                fabric.FabricName,
                $"{fabric.Supplier?.SupplierName ?? "-"} / {fabric.Color?.Name ?? "-"}",
                fabric.CurrentStockKg,
                "Kg",
                fabric.Status);
        }
    }

    private static IReadOnlyList<StockConsistencyRowDto> BuildStockConsistencyRows(
        IEnumerable<FabricMovement> fabricMovements,
        IEnumerable<Domain.Fabric.Fabric> fabrics,
        IEnumerable<Domain.Products.ProductVariant> variants,
        IEnumerable<SalesOrder> salesOrders,
        IEnumerable<Domain.Production.WarehouseEntry> warehouseEntries)
    {
        var fabricIncoming = fabricMovements
            .Where(movement => movement.MovementType is FabricMovementTypes.Purchase or FabricMovementTypes.Return or FabricMovementTypes.InventoryCount)
            .Sum(movement => movement.QuantityKg);

        var fabricOutgoing = fabricMovements
            .Where(movement => movement.MovementType is FabricMovementTypes.ProductionConsumption)
            .Sum(movement => Math.Abs(movement.QuantityKg));

        var completedSalesQuantity = salesOrders
            .Where(order => order.Status == SalesOrderStatuses.Completed)
            .Sum(order => order.Items.Sum(item => item.Quantity));

        var finishedGoodsIncoming = warehouseEntries.Sum(entry => entry.ActualQuantity);
        var productStock = variants.Sum(variant => variant.Stock);

        return
        [
            new StockConsistencyRowDto(
                "Kumaş",
                fabricIncoming,
                fabricOutgoing,
                fabrics.Sum(fabric => fabric.CurrentStockKg),
                "Kg",
                fabrics.Any(fabric => fabric.CurrentStockKg < 0) ? "Kontrol Gerekli" : "Tutarlı"),
            new StockConsistencyRowDto(
                "Ürün",
                finishedGoodsIncoming,
                completedSalesQuantity,
                productStock,
                "Adet",
                productStock < 0 ? "Kontrol Gerekli" : "Tutarlı"),
        ];
    }

    private static bool MatchesStatus(string value, string? status)
    {
        return string.IsNullOrWhiteSpace(status) || string.Equals(value, status, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesSearch(string? search, params string[] values)
    {
        return string.IsNullOrWhiteSpace(search)
            || values.Any(value => value.Contains(search, StringComparison.CurrentCultureIgnoreCase));
    }
}
