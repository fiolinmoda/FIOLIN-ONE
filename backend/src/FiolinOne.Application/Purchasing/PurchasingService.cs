using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Application.Common.Models;
using FiolinOne.Application.MasterData;
using FiolinOne.Domain.Purchasing;

namespace FiolinOne.Application.Purchasing;

public sealed class PurchasingService(
    IPurchasingRepository purchasingRepository,
    IMasterDataRepository masterDataRepository,
    IDocumentNumberGenerator documentNumberGenerator) : IPurchasingService
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
        var purchaseNumber = await GetDocumentNumberAsync(request.PurchaseNumber, DocumentNumberTypes.PurchaseOrder, cancellationToken);
        await EnsurePurchaseNumberIsUniqueAsync(purchaseNumber, null, cancellationToken);
        await EnsurePurchaseOrderItemMasterDataExistsAsync(request.Items, cancellationToken);

        var purchaseOrder = new PurchaseOrder(
            purchaseNumber,
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
        var receiptNumber = await GetDocumentNumberAsync(request.ReceiptNumber, DocumentNumberTypes.GoodsReceipt, cancellationToken);
        await EnsureReceiptNumberIsUniqueAsync(receiptNumber, null, cancellationToken);
        await EnsureGoodsReceiptDoesNotExceedOrderAsync(request.PurchaseOrderId, request.Items, null, cancellationToken);

        var goodsReceipt = new GoodsReceipt(
            receiptNumber,
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
        await EnsureGoodsReceiptDoesNotExceedOrderAsync(request.PurchaseOrderId, request.Items, id, cancellationToken);

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
        var invoiceNumber = await GetDocumentNumberAsync(request.InvoiceNumber, DocumentNumberTypes.PurchaseInvoice, cancellationToken);
        await EnsureInvoiceNumberIsUniqueAsync(invoiceNumber, null, cancellationToken);
        await EnsureInvoiceDoesNotExceedReceivedAsync(request.PurchaseOrderId, request.Items, null, cancellationToken);
        var invoiceItems = ToPurchaseInvoiceItems(Guid.Empty, request.Items);
        var invoiceAmount = invoiceItems.Sum(item => item.TotalAmount);

        var invoice = new PurchaseInvoice(
            invoiceNumber,
            ToUtc(request.InvoiceDate),
            request.SupplierId,
            request.PurchaseOrderId,
            invoiceAmount,
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
        await EnsureInvoiceDoesNotExceedReceivedAsync(request.PurchaseOrderId, request.Items, id, cancellationToken);
        var invoiceItems = ToPurchaseInvoiceItems(id, request.Items);
        var invoiceAmount = invoiceItems.Sum(item => item.TotalAmount);

        invoice.Update(
            request.InvoiceNumber.Trim(),
            ToUtc(request.InvoiceDate),
            request.SupplierId,
            request.PurchaseOrderId,
            invoiceAmount,
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await purchasingRepository.ReplacePurchaseInvoiceItemsAsync(invoice, invoiceItems, cancellationToken);
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
            throw new InvalidOperationException("Bu tedarikçi kodu zaten kullanılıyor.");
        }
    }

    private async Task EnsurePurchaseNumberIsUniqueAsync(string purchaseNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.PurchaseNumberExistsAsync(purchaseNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Bu satın alma numarası zaten kullanılıyor.");
        }
    }

    private async Task EnsureReceiptNumberIsUniqueAsync(string receiptNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.ReceiptNumberExistsAsync(receiptNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Bu mal kabul numarası zaten kullanılıyor.");
        }
    }

    private async Task EnsureInvoiceNumberIsUniqueAsync(string invoiceNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await purchasingRepository.InvoiceNumberExistsAsync(invoiceNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Bu fatura numarası zaten kullanılıyor.");
        }
    }

    private async Task EnsureSupplierExistsAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        if (!await purchasingRepository.SupplierExistsAsync(supplierId, cancellationToken))
        {
            throw new InvalidOperationException("Tedarikçi bulunamadı.");
        }
    }

    private async Task EnsurePurchaseOrderExistsAsync(Guid? purchaseOrderId, CancellationToken cancellationToken)
    {
        if (purchaseOrderId.HasValue &&
            !await purchasingRepository.PurchaseOrderExistsAsync(purchaseOrderId.Value, cancellationToken))
        {
            throw new InvalidOperationException("Satın alma siparişi bulunamadı.");
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
            throw new InvalidOperationException("Seçilen tanım kaydı bulunamadı.");
        }
    }

    private async Task EnsureGoodsReceiptDoesNotExceedOrderAsync(
        Guid? purchaseOrderId,
        IReadOnlyList<GoodsReceiptItemRequest> items,
        Guid? excludedReceiptId,
        CancellationToken cancellationToken)
    {
        foreach (var item in items.Where(item => item.PurchaseOrderItemId.HasValue))
        {
            var orderItem = await purchasingRepository.GetPurchaseOrderItemByIdAsync(item.PurchaseOrderItemId!.Value, cancellationToken)
                ?? throw new InvalidOperationException("Satın alma sipariş kalemi bulunamadı.");

            if (purchaseOrderId.HasValue && orderItem.PurchaseOrderId != purchaseOrderId.Value)
            {
                throw new InvalidOperationException("Mal kabul kalemi seçilen satın alma siparişine ait değil.");
            }

            var receivedBefore = await purchasingRepository.GetReceivedQuantityForPurchaseOrderItemAsync(
                orderItem.Id,
                excludedReceiptId,
                cancellationToken);

            if (receivedBefore + item.ReceivedQuantity > orderItem.Quantity)
            {
                throw new InvalidOperationException($"{orderItem.ItemName} için kabul miktarı sipariş miktarını aşamaz.");
            }
        }
    }

    private async Task EnsureInvoiceDoesNotExceedReceivedAsync(
        Guid? purchaseOrderId,
        IReadOnlyList<PurchaseInvoiceItemRequest> items,
        Guid? excludedInvoiceId,
        CancellationToken cancellationToken)
    {
        foreach (var item in items.Where(item => item.PurchaseOrderItemId.HasValue))
        {
            var orderItem = await purchasingRepository.GetPurchaseOrderItemByIdAsync(item.PurchaseOrderItemId!.Value, cancellationToken)
                ?? throw new InvalidOperationException("Satın alma sipariş kalemi bulunamadı.");

            if (purchaseOrderId.HasValue && orderItem.PurchaseOrderId != purchaseOrderId.Value)
            {
                throw new InvalidOperationException("Fatura kalemi seçilen satın alma siparişine ait değil.");
            }

            var receivedQuantity = await purchasingRepository.GetReceivedQuantityForPurchaseOrderItemAsync(
                orderItem.Id,
                null,
                cancellationToken);
            var invoicedBefore = await purchasingRepository.GetInvoicedQuantityForPurchaseOrderItemAsync(
                orderItem.Id,
                excludedInvoiceId,
                cancellationToken);

            if (invoicedBefore + item.Quantity > receivedQuantity)
            {
                throw new InvalidOperationException($"{orderItem.ItemName} için fatura miktarı kabul edilmiş miktarı aşamaz.");
            }
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
            request.Quantity * request.UnitPrice)).ToList();
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

    private async Task<string> GetDocumentNumberAsync(string? requestedNumber, string documentType, CancellationToken cancellationToken)
    {
        return string.IsNullOrWhiteSpace(requestedNumber)
            ? await documentNumberGenerator.GenerateAsync(documentType, cancellationToken)
            : requestedNumber.Trim();
    }
}
