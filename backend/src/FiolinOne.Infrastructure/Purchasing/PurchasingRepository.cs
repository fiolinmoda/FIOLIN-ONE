using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Purchasing;
using FiolinOne.Domain.Purchasing;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Purchasing;

public sealed class PurchasingRepository(ApplicationDbContext dbContext) : IPurchasingRepository
{
    public async Task<PagedResult<Supplier>> GetSuppliersAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = dbContext.Suppliers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(supplier =>
                supplier.SupplierCode.ToLower().Contains(term) ||
                supplier.SupplierName.ToLower().Contains(term) ||
                (supplier.Phone != null && supplier.Phone.ToLower().Contains(term)) ||
                (supplier.Email != null && supplier.Email.ToLower().Contains(term)) ||
                (supplier.TaxNumber != null && supplier.TaxNumber.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && bool.TryParse(query.Status, out var active))
        {
            source = source.Where(supplier => supplier.Active == active);
        }

        source = ApplySupplierSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<Supplier?> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Suppliers.FirstOrDefaultAsync(supplier => supplier.Id == id, cancellationToken);
    }

    public Task<bool> SupplierExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Suppliers.AnyAsync(supplier => supplier.Id == id, cancellationToken);
    }

    public Task<bool> SupplierCodeExistsAsync(string supplierCode, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.Suppliers.AnyAsync(
            supplier => supplier.SupplierCode == supplierCode && (!excludedId.HasValue || supplier.Id != excludedId),
            cancellationToken);
    }

    public async Task AddSupplierAsync(Supplier supplier, CancellationToken cancellationToken)
    {
        await dbContext.Suppliers.AddAsync(supplier, cancellationToken);
    }

    public async Task<PagedResult<PurchaseOrder>> GetPurchaseOrdersAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = dbContext.PurchaseOrders
            .AsNoTracking()
            .Include(order => order.Supplier)
            .Include(order => order.Items)
            .ThenInclude(item => item.FabricType)
            .Include(order => order.Items)
            .ThenInclude(item => item.Color)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(order =>
                order.PurchaseNumber.ToLower().Contains(term) ||
                order.Supplier!.SupplierName.ToLower().Contains(term) ||
                order.Items.Any(item => item.ItemName.ToLower().Contains(term)));
        }

        source = ApplyStatusFilter(source, query.Status);
        source = ApplyPurchaseOrderSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PurchaseOrders
            .Include(order => order.Supplier)
            .Include(order => order.Items)
            .ThenInclude(item => item.FabricType)
            .Include(order => order.Items)
            .ThenInclude(item => item.Color)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public Task<bool> PurchaseOrderExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PurchaseOrders.AnyAsync(order => order.Id == id, cancellationToken);
    }

    public Task<bool> PurchaseNumberExistsAsync(string purchaseNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.PurchaseOrders.AnyAsync(
            order => order.PurchaseNumber == purchaseNumber && (!excludedId.HasValue || order.Id != excludedId),
            cancellationToken);
    }

    public async Task AddPurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken)
    {
        await dbContext.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
    }

    public async Task ReplacePurchaseOrderItemsAsync(
        PurchaseOrder purchaseOrder,
        IReadOnlyList<PurchaseOrderItem> items,
        CancellationToken cancellationToken)
    {
        var existingItems = await dbContext.PurchaseOrderItems
            .Where(item => item.PurchaseOrderId == purchaseOrder.Id)
            .ToListAsync(cancellationToken);

        foreach (var item in existingItems)
        {
            item.SoftDelete();
        }

        await dbContext.PurchaseOrderItems.AddRangeAsync(items, cancellationToken);
    }

    public async Task<PagedResult<GoodsReceipt>> GetGoodsReceiptsAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = dbContext.GoodsReceipts
            .AsNoTracking()
            .Include(receipt => receipt.Supplier)
            .Include(receipt => receipt.PurchaseOrder)
            .Include(receipt => receipt.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(receipt =>
                receipt.ReceiptNumber.ToLower().Contains(term) ||
                receipt.Supplier!.SupplierName.ToLower().Contains(term) ||
                receipt.Warehouse.ToLower().Contains(term) ||
                receipt.Items.Any(item => item.ItemName.ToLower().Contains(term)));
        }

        source = ApplyStatusFilter(source, query.Status);
        source = ApplyGoodsReceiptSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<GoodsReceipt?> GetGoodsReceiptByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.GoodsReceipts
            .Include(receipt => receipt.Supplier)
            .Include(receipt => receipt.PurchaseOrder)
            .Include(receipt => receipt.Items)
            .FirstOrDefaultAsync(receipt => receipt.Id == id, cancellationToken);
    }

    public Task<bool> ReceiptNumberExistsAsync(string receiptNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.GoodsReceipts.AnyAsync(
            receipt => receipt.ReceiptNumber == receiptNumber && (!excludedId.HasValue || receipt.Id != excludedId),
            cancellationToken);
    }

    public async Task AddGoodsReceiptAsync(GoodsReceipt goodsReceipt, CancellationToken cancellationToken)
    {
        await dbContext.GoodsReceipts.AddAsync(goodsReceipt, cancellationToken);
    }

    public async Task ReplaceGoodsReceiptItemsAsync(
        GoodsReceipt goodsReceipt,
        IReadOnlyList<GoodsReceiptItem> items,
        CancellationToken cancellationToken)
    {
        var existingItems = await dbContext.GoodsReceiptItems
            .Where(item => item.GoodsReceiptId == goodsReceipt.Id)
            .ToListAsync(cancellationToken);

        foreach (var item in existingItems)
        {
            item.SoftDelete();
        }

        await dbContext.GoodsReceiptItems.AddRangeAsync(items, cancellationToken);
    }

    public async Task<PagedResult<PurchaseInvoice>> GetPurchaseInvoicesAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = dbContext.PurchaseInvoices
            .AsNoTracking()
            .Include(invoice => invoice.Supplier)
            .Include(invoice => invoice.PurchaseOrder)
            .Include(invoice => invoice.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(invoice =>
                invoice.InvoiceNumber.ToLower().Contains(term) ||
                invoice.Supplier!.SupplierName.ToLower().Contains(term) ||
                invoice.Items.Any(item => item.ItemName.ToLower().Contains(term)));
        }

        source = ApplyStatusFilter(source, query.Status);
        source = ApplyPurchaseInvoiceSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PurchaseInvoices
            .Include(invoice => invoice.Supplier)
            .Include(invoice => invoice.PurchaseOrder)
            .Include(invoice => invoice.Items)
            .FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
    }

    public Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.PurchaseInvoices.AnyAsync(
            invoice => invoice.InvoiceNumber == invoiceNumber && (!excludedId.HasValue || invoice.Id != excludedId),
            cancellationToken);
    }

    public async Task AddPurchaseInvoiceAsync(PurchaseInvoice purchaseInvoice, CancellationToken cancellationToken)
    {
        await dbContext.PurchaseInvoices.AddAsync(purchaseInvoice, cancellationToken);
    }

    public async Task ReplacePurchaseInvoiceItemsAsync(
        PurchaseInvoice purchaseInvoice,
        IReadOnlyList<PurchaseInvoiceItem> items,
        CancellationToken cancellationToken)
    {
        var existingItems = await dbContext.PurchaseInvoiceItems
            .Where(item => item.PurchaseInvoiceId == purchaseInvoice.Id)
            .ToListAsync(cancellationToken);

        foreach (var item in existingItems)
        {
            item.SoftDelete();
        }

        await dbContext.PurchaseInvoiceItems.AddRangeAsync(items, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<T> ApplyStatusFilter<T>(IQueryable<T> source, string? status)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return source;
        }

        return source.Where(entity => EF.Property<string>(entity, "Status") == status.Trim());
    }

    private static IQueryable<Supplier> ApplySupplierSorting(IQueryable<Supplier> source, string? sortBy, string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "suppliername" => descending ? source.OrderByDescending(supplier => supplier.SupplierName) : source.OrderBy(supplier => supplier.SupplierName),
            "createdat" => descending ? source.OrderByDescending(supplier => supplier.CreatedAtUtc) : source.OrderBy(supplier => supplier.CreatedAtUtc),
            _ => descending ? source.OrderByDescending(supplier => supplier.SupplierCode) : source.OrderBy(supplier => supplier.SupplierCode)
        };
    }

    private static IQueryable<PurchaseOrder> ApplyPurchaseOrderSorting(IQueryable<PurchaseOrder> source, string? sortBy, string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "supplier" => descending ? source.OrderByDescending(order => order.Supplier!.SupplierName) : source.OrderBy(order => order.Supplier!.SupplierName),
            "orderdate" => descending ? source.OrderByDescending(order => order.OrderDate) : source.OrderBy(order => order.OrderDate),
            "expecteddate" => descending ? source.OrderByDescending(order => order.ExpectedDate) : source.OrderBy(order => order.ExpectedDate),
            "status" => descending ? source.OrderByDescending(order => order.Status) : source.OrderBy(order => order.Status),
            _ => descending ? source.OrderByDescending(order => order.PurchaseNumber) : source.OrderBy(order => order.PurchaseNumber)
        };
    }

    private static IQueryable<GoodsReceipt> ApplyGoodsReceiptSorting(IQueryable<GoodsReceipt> source, string? sortBy, string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "supplier" => descending ? source.OrderByDescending(receipt => receipt.Supplier!.SupplierName) : source.OrderBy(receipt => receipt.Supplier!.SupplierName),
            "receiptdate" => descending ? source.OrderByDescending(receipt => receipt.ReceiptDate) : source.OrderBy(receipt => receipt.ReceiptDate),
            "warehouse" => descending ? source.OrderByDescending(receipt => receipt.Warehouse) : source.OrderBy(receipt => receipt.Warehouse),
            "status" => descending ? source.OrderByDescending(receipt => receipt.Status) : source.OrderBy(receipt => receipt.Status),
            _ => descending ? source.OrderByDescending(receipt => receipt.ReceiptNumber) : source.OrderBy(receipt => receipt.ReceiptNumber)
        };
    }

    private static IQueryable<PurchaseInvoice> ApplyPurchaseInvoiceSorting(IQueryable<PurchaseInvoice> source, string? sortBy, string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "supplier" => descending ? source.OrderByDescending(invoice => invoice.Supplier!.SupplierName) : source.OrderBy(invoice => invoice.Supplier!.SupplierName),
            "invoicedate" => descending ? source.OrderByDescending(invoice => invoice.InvoiceDate) : source.OrderBy(invoice => invoice.InvoiceDate),
            "invoiceamount" => descending ? source.OrderByDescending(invoice => invoice.InvoiceAmount) : source.OrderBy(invoice => invoice.InvoiceAmount),
            "status" => descending ? source.OrderByDescending(invoice => invoice.Status) : source.OrderBy(invoice => invoice.Status),
            _ => descending ? source.OrderByDescending(invoice => invoice.InvoiceNumber) : source.OrderBy(invoice => invoice.InvoiceNumber)
        };
    }

    private static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        IQueryable<T> source,
        QueryParameters query,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var totalItems = await source.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, page, pageSize, totalItems, totalPages);
    }

    private static bool IsDescending(string? sortDirection)
    {
        return string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSort(string? sortBy)
    {
        return sortBy?.Trim().Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant() ?? string.Empty;
    }
}
