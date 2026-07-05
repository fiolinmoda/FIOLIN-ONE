using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Purchasing;

public sealed class PurchaseInvoiceItem : AuditableEntity
{
    private PurchaseInvoiceItem()
    {
    }

    public PurchaseInvoiceItem(
        Guid purchaseInvoiceId,
        Guid? purchaseOrderItemId,
        string itemName,
        decimal quantity,
        string unit,
        decimal unitPrice,
        decimal totalAmount)
    {
        PurchaseInvoiceId = purchaseInvoiceId;
        PurchaseOrderItemId = purchaseOrderItemId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        TotalAmount = totalAmount;
    }

    public Guid PurchaseInvoiceId { get; private set; }
    public Guid? PurchaseOrderItemId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public PurchaseInvoice? PurchaseInvoice { get; private set; }
    public PurchaseOrderItem? PurchaseOrderItem { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        Guid? purchaseOrderItemId,
        string itemName,
        decimal quantity,
        string unit,
        decimal unitPrice,
        decimal totalAmount)
    {
        PurchaseOrderItemId = purchaseOrderItemId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        TotalAmount = totalAmount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
