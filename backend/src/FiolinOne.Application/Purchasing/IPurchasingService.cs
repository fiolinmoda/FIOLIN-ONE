using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Purchasing;

public interface IPurchasingService
{
    Task<PagedResult<SupplierDto>> GetSuppliersAsync(PurchasingQuery query, CancellationToken cancellationToken);
    Task<SupplierDto?> GetSupplierAsync(Guid id, CancellationToken cancellationToken);
    Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken);
    Task<SupplierDto?> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(PurchasingQuery query, CancellationToken cancellationToken);
    Task<PurchaseOrderDto?> GetPurchaseOrderAsync(Guid id, CancellationToken cancellationToken);
    Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken);
    Task<PurchaseOrderDto?> UpdatePurchaseOrderAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken);
    Task<bool> DeletePurchaseOrderAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<GoodsReceiptDto>> GetGoodsReceiptsAsync(PurchasingQuery query, CancellationToken cancellationToken);
    Task<GoodsReceiptDto?> GetGoodsReceiptAsync(Guid id, CancellationToken cancellationToken);
    Task<GoodsReceiptDto> CreateGoodsReceiptAsync(CreateGoodsReceiptRequest request, CancellationToken cancellationToken);
    Task<GoodsReceiptDto?> UpdateGoodsReceiptAsync(Guid id, UpdateGoodsReceiptRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteGoodsReceiptAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<PurchaseInvoiceDto>> GetPurchaseInvoicesAsync(PurchasingQuery query, CancellationToken cancellationToken);
    Task<PurchaseInvoiceDto?> GetPurchaseInvoiceAsync(Guid id, CancellationToken cancellationToken);
    Task<PurchaseInvoiceDto> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceRequest request, CancellationToken cancellationToken);
    Task<PurchaseInvoiceDto?> UpdatePurchaseInvoiceAsync(Guid id, UpdatePurchaseInvoiceRequest request, CancellationToken cancellationToken);
    Task<bool> DeletePurchaseInvoiceAsync(Guid id, CancellationToken cancellationToken);
}
