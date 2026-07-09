using FiolinOne.Application.Common.Models;
using FiolinOne.Domain.Purchasing;

namespace FiolinOne.Application.Purchasing;

public interface IPurchasingRepository
{
    Task<PagedResult<Supplier>> GetSuppliersAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<Supplier?> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> SupplierExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> SupplierCodeExistsAsync(string supplierCode, Guid? excludedId, CancellationToken cancellationToken);
    Task AddSupplierAsync(Supplier supplier, CancellationToken cancellationToken);

    Task<PagedResult<PurchaseOrder>> GetPurchaseOrdersAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PurchaseOrderItem?> GetPurchaseOrderItemByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> PurchaseOrderExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> PurchaseNumberExistsAsync(string purchaseNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task AddPurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken);
    Task ReplacePurchaseOrderItemsAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderItem> items, CancellationToken cancellationToken);

    Task<PagedResult<GoodsReceipt>> GetGoodsReceiptsAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<GoodsReceipt?> GetGoodsReceiptByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ReceiptNumberExistsAsync(string receiptNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task<decimal> GetReceivedQuantityForPurchaseOrderItemAsync(Guid purchaseOrderItemId, Guid? excludedReceiptId, CancellationToken cancellationToken);
    Task AddGoodsReceiptAsync(GoodsReceipt goodsReceipt, CancellationToken cancellationToken);
    Task ReplaceGoodsReceiptItemsAsync(GoodsReceipt goodsReceipt, IReadOnlyList<GoodsReceiptItem> items, CancellationToken cancellationToken);

    Task<PagedResult<PurchaseInvoice>> GetPurchaseInvoicesAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task<decimal> GetInvoicedQuantityForPurchaseOrderItemAsync(Guid purchaseOrderItemId, Guid? excludedInvoiceId, CancellationToken cancellationToken);
    Task AddPurchaseInvoiceAsync(PurchaseInvoice purchaseInvoice, CancellationToken cancellationToken);
    Task ReplacePurchaseInvoiceItemsAsync(PurchaseInvoice purchaseInvoice, IReadOnlyList<PurchaseInvoiceItem> items, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
