using FiolinOne.Application.Common.Models;
using FiolinOne.Application.MasterData;
using FiolinOne.Domain.Purchasing;

namespace FiolinOne.Application.Purchasing;

public sealed class PurchasingService(
    IPurchasingRepository purchasingRepository,
    IMasterDataRepository masterDataRepository) : IPurchasingService
{
    public async Task<PagedResult<SupplierDto>> GetSuppliersAsync(PurchasingQuery query, CancellationToken cancellationToken)
    {
        var result = await purchasingRepository.GetSuppliersAsync(query.ToParameters(), cancellationToken);

        return MapPaged(result, ToDto);
    }

    public async Task<SupplierDto?> GetSupplierAsync(Guid id, CancellationToken cancellationToken)
    {
        var supplier = await purchasingRepository.GetSupplierByIdAsync(id, cancellationToken);

        return supplier is null ? null : ToDto(supplier);
    }

    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        await EnsureSupplierCodeIsUniqueAsync(request.SupplierCode, null, cancellationToken);

        var supplier = new Supplier(
            request.SupplierCode.Trim(),
            request.SupplierName.Trim(),
            NormalizeOptional(request.Phone),
            NormalizeOptional(request.Email),
            NormalizeOptional(request.Address),
            NormalizeOptional(request.TaxNumber),
            NormalizeOptional(request.PaymentTerm),
            request.Active);

        await purchasingRepository.AddSupplierAsync(supplier, cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        return ToDto(supplier);
    }

    public async Task<SupplierDto?> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await purchasingRepository.GetSupplierByIdAsync(id, cancellationToken);

        if (supplier is null)
        {
            return null;
        }

        await EnsureSupplierCodeIsUniqueAsync(request.SupplierCode, id, cancellationToken);

        supplier.Update(
            request.SupplierCode.Trim(),
            request.SupplierName.Trim(),
            NormalizeOptional(request.Phone),
            NormalizeOptional(request.Email),
            NormalizeOptional(request.Address),
            NormalizeOptional(request.TaxNumber),
            NormalizeOptional(request.PaymentTerm),
            request.Active);

        await purchasingRepository.SaveChangesAsync(cancellationToken);

        return ToDto(supplier);
    }

    public async Task<bool> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken)
    {
        var supplier = await purchasingRepository.GetSupplierByIdAsync(id, cancellationToken);

        if (supplier is null)
        {
            return false;
        }

        supplier.SoftDelete();
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(PurchasingQuery query, CancellationToken cancellationToken)
    {
        var result = await purchasingRepository.GetPurchaseOrdersAsync(query.ToParameters(), cancellationToken);

        return MapPaged(result, ToDto);
    }

    public async Task<PurchaseOrderDto?> GetPurchaseOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchasingRepository.GetPurchaseOrderByIdAsync(id, cancellationToken);

        return purchaseOrder is null ? null : ToDto(purchaseOrder);
    }

    public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseNumberIsUniqueAsync(request.PurchaseNumber, null, cancellationToken);
        await EnsurePurchaseOrderItemMasterDataExistsAsync(request.Items, cancellationToken);

        var purchaseOrder = new PurchaseOrder(
            request.PurchaseNumber.Trim(),
            request.SupplierId,
            ToUtc(request.OrderDate),
            ToUtc(request.ExpectedDate),
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.AddPurchaseOrderAsync(purchaseOrder, cancellationToken);
        await purchasingRepository.ReplacePurchaseOrderItemsAsync(purchaseOrder, ToPurchaseOrderItems(purchaseOrder.Id, request.Items), cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        purchaseOrder = await purchasingRepository.GetPurchaseOrderByIdAsync(purchaseOrder.Id, cancellationToken) ?? purchaseOrder;

        return ToDto(purchaseOrder);
    }

    public async Task<PurchaseOrderDto?> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchasingRepository.GetPurchaseOrderByIdAsync(id, cancellationToken);

        if (purchaseOrder is null)
        {
            return null;
        }

        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseNumberIsUniqueAsync(request.PurchaseNumber, id, cancellationToken);
        await EnsurePurchaseOrderItemMasterDataExistsAsync(request.Items, cancellationToken);

        purchaseOrder.Update(
            request.PurchaseNumber.Trim(),
            request.SupplierId,
            ToUtc(request.OrderDate),
            ToUtc(request.ExpectedDate),
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.ReplacePurchaseOrderItemsAsync(purchaseOrder, ToPurchaseOrderItems(id, request.Items), cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        purchaseOrder = await purchasingRepository.GetPurchaseOrderByIdAsync(id, cancellationToken) ?? purchaseOrder;

        return ToDto(purchaseOrder);
    }

    public async Task<bool> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchasingRepository.GetPurchaseOrderByIdAsync(id, cancellationToken);

        if (purchaseOrder is null)
        {
            return false;
        }

        purchaseOrder.SoftDelete();
        foreach (var item in purchaseOrder.Items)
        {
            item.SoftDelete();
        }

        await purchasingRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<PagedResult<GoodsReceiptDto>> GetGoodsReceiptsAsync(PurchasingQuery query, CancellationToken cancellationToken)
    {
        var result = await purchasingRepository.GetGoodsReceiptsAsync(query.ToParameters(), cancellationToken);

        return MapPaged(result, ToDto);
    }

    public async Task<GoodsReceiptDto?> GetGoodsReceiptAsync(Guid id, CancellationToken cancellationToken)
    {
        var goodsReceipt = await purchasingRepository.GetGoodsReceiptByIdAsync(id, cancellationToken);

        return goodsReceipt is null ? null : ToDto(goodsReceipt);
    }

    public async Task<GoodsReceiptDto> CreateGoodsReceiptAsync(CreateGoodsReceiptRequest request, CancellationToken cancellationToken)
    {
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseOrderExistsAsync(request.PurchaseOrderId, cancellationToken);
        await EnsureReceiptNumberIsUniqueAsync(request.ReceiptNumber, null, cancellationToken);

        var goodsReceipt = new GoodsReceipt(
            request.ReceiptNumber.Trim(),
            request.SupplierId,
            request.PurchaseOrderId,
            ToUtc(request.ReceiptDate),
            request.Warehouse.Trim(),
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.AddGoodsReceiptAsync(goodsReceipt, cancellationToken);
        await purchasingRepository.ReplaceGoodsReceiptItemsAsync(goodsReceipt, ToGoodsReceiptItems(goodsReceipt.Id, request.Items), cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        goodsReceipt = await purchasingRepository.GetGoodsReceiptByIdAsync(goodsReceipt.Id, cancellationToken) ?? goodsReceipt;

        return ToDto(goodsReceipt);
    }

    public async Task<GoodsReceiptDto?> UpdateGoodsReceiptAsync(Guid id, UpdateGoodsReceiptRequest request, CancellationToken cancellationToken)
    {
        var goodsReceipt = await purchasingRepository.GetGoodsReceiptByIdAsync(id, cancellationToken);

        if (goodsReceipt is null)
        {
            return null;
        }

        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseOrderExistsAsync(request.PurchaseOrderId, cancellationToken);
        await EnsureReceiptNumberIsUniqueAsync(request.ReceiptNumber, id, cancellationToken);

        goodsReceipt.Update(
            request.ReceiptNumber.Trim(),
            request.SupplierId,
            request.PurchaseOrderId,
            ToUtc(request.ReceiptDate),
            request.Warehouse.Trim(),
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.ReplaceGoodsReceiptItemsAsync(goodsReceipt, ToGoodsReceiptItems(id, request.Items), cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        goodsReceipt = await purchasingRepository.GetGoodsReceiptByIdAsync(id, cancellationToken) ?? goodsReceipt;

        return ToDto(goodsReceipt);
    }

    public async Task<bool> DeleteGoodsReceiptAsync(Guid id, CancellationToken cancellationToken)
    {
        var goodsReceipt = await purchasingRepository.GetGoodsReceiptByIdAsync(id, cancellationToken);

        if (goodsReceipt is null)
        {
            return false;
        }

        goodsReceipt.SoftDelete();
        foreach (var item in goodsReceipt.Items)
        {
            item.SoftDelete();
        }

        await purchasingRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<PagedResult<PurchaseInvoiceDto>> GetPurchaseInvoicesAsync(PurchasingQuery query, CancellationToken cancellationToken)
    {
        var result = await purchasingRepository.GetPurchaseInvoicesAsync(query.ToParameters(), cancellationToken);

        return MapPaged(result, ToDto);
    }

    public async Task<PurchaseInvoiceDto?> GetPurchaseInvoiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await purchasingRepository.GetPurchaseInvoiceByIdAsync(id, cancellationToken);

        return invoice is null ? null : ToDto(invoice);
    }

    public async Task<PurchaseInvoiceDto> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceRequest request, CancellationToken cancellationToken)
    {
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseOrderExistsAsync(request.PurchaseOrderId, cancellationToken);
        await EnsureInvoiceNumberIsUniqueAsync(request.InvoiceNumber, null, cancellationToken);

        var invoice = new PurchaseInvoice(
            request.InvoiceNumber.Trim(),
            ToUtc(request.InvoiceDate),
            request.SupplierId,
            request.PurchaseOrderId,
            request.InvoiceAmount,
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.AddPurchaseInvoiceAsync(invoice, cancellationToken);
        await purchasingRepository.ReplacePurchaseInvoiceItemsAsync(invoice, ToPurchaseInvoiceItems(invoice.Id, request.Items), cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        invoice = await purchasingRepository.GetPurchaseInvoiceByIdAsync(invoice.Id, cancellationToken) ?? invoice;

        return ToDto(invoice);
    }

    public async Task<PurchaseInvoiceDto?> UpdatePurchaseInvoiceAsync(Guid id, UpdatePurchaseInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await purchasingRepository.GetPurchaseInvoiceByIdAsync(id, cancellationToken);

        if (invoice is null)
        {
            return null;
        }

        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseOrderExistsAsync(request.PurchaseOrderId, cancellationToken);
        await EnsureInvoiceNumberIsUniqueAsync(request.InvoiceNumber, id, cancellationToken);

        invoice.Update(
            request.InvoiceNumber.Trim(),
            ToUtc(request.InvoiceDate),
            request.SupplierId,
            request.PurchaseOrderId,
            request.InvoiceAmount,
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.ReplacePurchaseInvoiceItemsAsync(invoice, ToPurchaseInvoiceItems(id, request.Items), cancellationToken);
        await purchasingRepository.SaveChangesAsync(cancellationToken);

        invoice = await purchasingRepository.GetPurchaseInvoiceByIdAsync(id, cancellationToken) ?? invoice;

        return ToDto(invoice);
    }

    public async Task<bool> DeletePurchaseInvoiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await purchasingRepository.GetPurchaseInvoiceByIdAsync(id, cancellationToken);

        if (invoice is null)
        {
            return false;
        }

        invoice.SoftDelete();
        foreach (var item in invoice.Items)
        {
            item.SoftDelete();
        }

        await purchasingRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureSupplierCodeIsUniqueAsync(string supplierCode, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.SupplierCodeExistsAsync(supplierCode.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Supplier code already exists.");
        }
    }

    private async Task EnsurePurchaseNumberIsUniqueAsync(string purchaseNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.PurchaseNumberExistsAsync(purchaseNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Purchase number already exists.");
        }
    }

    private async Task EnsureReceiptNumberIsUniqueAsync(string receiptNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.ReceiptNumberExistsAsync(receiptNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Receipt number already exists.");
        }
    }

    private async Task EnsureInvoiceNumberIsUniqueAsync(string invoiceNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.InvoiceNumberExistsAsync(invoiceNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Invoice number already exists.");
        }
    }

    private async Task EnsureSupplierExistsAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        if (!await purchasingRepository.SupplierExistsAsync(supplierId, cancellationToken))
        {
            throw new InvalidOperationException("Supplier does not exist.");
        }
    }

    private async Task EnsurePurchaseOrderExistsAsync(Guid? purchaseOrderId, CancellationToken cancellationToken)
    {
        if (purchaseOrderId.HasValue &&
            !await purchasingRepository.PurchaseOrderExistsAsync(purchaseOrderId.Value, cancellationToken))
        {
            throw new InvalidOperationException("Purchase order does not exist.");
        }
    }

    private async Task EnsurePurchaseOrderItemMasterDataExistsAsync(
        IReadOnlyList<PurchaseOrderItemRequest> items,
        CancellationToken cancellationToken)
    {
        foreach (var item in items)
        {
            await EnsureMasterDataExistsAsync(item.FabricTypeId, "fabric-types", cancellationToken);
            await EnsureMasterDataExistsAsync(item.ColorId, "colors", cancellationToken);
        }
    }

    private async Task EnsureMasterDataExistsAsync(Guid? id, string type, CancellationToken cancellationToken)
    {
        if (id.HasValue && !await masterDataRepository.ExistsAsync(type, id.Value, cancellationToken))
        {
            throw new InvalidOperationException("Selected master data item does not exist.");
        }
    }

    private static IReadOnlyList<PurchaseOrderItem> ToPurchaseOrderItems(
        Guid purchaseOrderId,
        IReadOnlyList<PurchaseOrderItemRequest> requests)
    {
        return requests.Select(request => new PurchaseOrderItem(
            purchaseOrderId,
            request.FabricTypeId,
            request.ColorId,
            request.ItemName.Trim(),
            request.Quantity,
            request.Unit.Trim(),
            request.UnitPrice,
            request.ReceivedQuantity,
            request.Status.Trim())).ToList();
    }

    private static IReadOnlyList<GoodsReceiptItem> ToGoodsReceiptItems(
        Guid goodsReceiptId,
        IReadOnlyList<GoodsReceiptItemRequest> requests)
    {
        return requests.Select(request => new GoodsReceiptItem(
            goodsReceiptId,
            request.PurchaseOrderItemId,
            request.ItemName.Trim(),
            request.ReceivedQuantity,
            request.Unit.Trim(),
            request.Acceptance.Trim(),
            request.DifferenceQuantity)).ToList();
    }

    private static IReadOnlyList<PurchaseInvoiceItem> ToPurchaseInvoiceItems(
        Guid purchaseInvoiceId,
        IReadOnlyList<PurchaseInvoiceItemRequest> requests)
    {
        return requests.Select(request => new PurchaseInvoiceItem(
            purchaseInvoiceId,
            request.PurchaseOrderItemId,
            request.ItemName.Trim(),
            request.Quantity,
            request.Unit.Trim(),
            request.UnitPrice,
            request.TotalAmount)).ToList();
    }

    private static SupplierDto ToDto(Supplier supplier)
    {
        return new SupplierDto(
            supplier.Id,
            supplier.SupplierCode,
            supplier.SupplierName,
            supplier.Phone,
            supplier.Email,
            supplier.Address,
            supplier.TaxNumber,
            supplier.PaymentTerm,
            supplier.Active,
            supplier.CreatedAt,
            supplier.UpdatedAt);
    }

    private static PurchaseOrderDto ToDto(PurchaseOrder purchaseOrder)
    {
        var items = purchaseOrder.Items
            .Where(item => !item.IsDeleted)
            .Select(ToDto)
            .ToList();

        return new PurchaseOrderDto(
            purchaseOrder.Id,
            purchaseOrder.PurchaseNumber,
            purchaseOrder.SupplierId,
            purchaseOrder.Supplier?.SupplierName ?? string.Empty,
            purchaseOrder.OrderDate,
            purchaseOrder.ExpectedDate,
            purchaseOrder.Status,
            purchaseOrder.Notes,
            items.Sum(item => item.Quantity * item.UnitPrice),
            items,
            purchaseOrder.CreatedAt,
            purchaseOrder.UpdatedAt);
    }

    private static PurchaseOrderItemDto ToDto(PurchaseOrderItem item)
    {
        return new PurchaseOrderItemDto(
            item.Id,
            item.PurchaseOrderId,
            item.FabricTypeId,
            item.FabricType?.Name,
            item.ColorId,
            item.Color?.Name,
            item.ItemName,
            item.Quantity,
            item.Unit,
            item.UnitPrice,
            item.ReceivedQuantity,
            item.RemainingQuantity,
            item.Status);
    }

    private static GoodsReceiptDto ToDto(GoodsReceipt goodsReceipt)
    {
        return new GoodsReceiptDto(
            goodsReceipt.Id,
            goodsReceipt.ReceiptNumber,
            goodsReceipt.SupplierId,
            goodsReceipt.Supplier?.SupplierName ?? string.Empty,
            goodsReceipt.PurchaseOrderId,
            goodsReceipt.PurchaseOrder?.PurchaseNumber,
            goodsReceipt.ReceiptDate,
            goodsReceipt.Warehouse,
            goodsReceipt.Status,
            goodsReceipt.Notes,
            goodsReceipt.Items.Where(item => !item.IsDeleted).Select(ToDto).ToList(),
            goodsReceipt.CreatedAt,
            goodsReceipt.UpdatedAt);
    }

    private static GoodsReceiptItemDto ToDto(GoodsReceiptItem item)
    {
        return new GoodsReceiptItemDto(
            item.Id,
            item.GoodsReceiptId,
            item.PurchaseOrderItemId,
            item.ItemName,
            item.ReceivedQuantity,
            item.Unit,
            item.Acceptance,
            item.DifferenceQuantity);
    }

    private static PurchaseInvoiceDto ToDto(PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.InvoiceDate,
            invoice.SupplierId,
            invoice.Supplier?.SupplierName ?? string.Empty,
            invoice.PurchaseOrderId,
            invoice.PurchaseOrder?.PurchaseNumber,
            invoice.InvoiceAmount,
            invoice.Status,
            invoice.Notes,
            invoice.Items.Where(item => !item.IsDeleted).Select(ToDto).ToList(),
            invoice.CreatedAt,
            invoice.UpdatedAt);
    }

    private static PurchaseInvoiceItemDto ToDto(PurchaseInvoiceItem item)
    {
        return new PurchaseInvoiceItemDto(
            item.Id,
            item.PurchaseInvoiceId,
            item.PurchaseOrderItemId,
            item.ItemName,
            item.Quantity,
            item.Unit,
            item.UnitPrice,
            item.TotalAmount);
    }

    private static PagedResult<TTarget> MapPaged<TSource, TTarget>(
        PagedResult<TSource> source,
        Func<TSource, TTarget> map)
    {
        return new PagedResult<TTarget>(
            source.Items.Select(map).ToList(),
            source.Page,
            source.PageSize,
            source.TotalItems,
            source.TotalPages);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static DateTime? ToUtc(DateTime? value)
    {
        return value.HasValue ? ToUtc(value.Value) : null;
    }
}
