using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Purchasing;

public sealed class GoodsReceiptItem : AuditableEntity
{
    private GoodsReceiptItem()
    {
    }

    public GoodsReceiptItem(
        Guid goodsReceiptId,
        Guid? purchaseOrderItemId,
        string itemName,
        decimal receivedQuantity,
        string unit,
        string acceptance,
        decimal differenceQuantity)
    {
        GoodsReceiptId = goodsReceiptId;
        PurchaseOrderItemId = purchaseOrderItemId;
        ItemName = itemName;
        ReceivedQuantity = receivedQuantity;
        Unit = unit;
        Acceptance = acceptance;
        DifferenceQuantity = differenceQuantity;
    }

    public Guid GoodsReceiptId { get; private set; }
    public Guid? PurchaseOrderItemId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public decimal ReceivedQuantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public string Acceptance { get; private set; } = string.Empty;
    public decimal DifferenceQuantity { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public GoodsReceipt? GoodsReceipt { get; private set; }
    public PurchaseOrderItem? PurchaseOrderItem { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        Guid? purchaseOrderItemId,
        string itemName,
        decimal receivedQuantity,
        string unit,
        string acceptance,
        decimal differenceQuantity)
    {
        PurchaseOrderItemId = purchaseOrderItemId;
        ItemName = itemName;
        ReceivedQuantity = receivedQuantity;
        Unit = unit;
        Acceptance = acceptance;
        DifferenceQuantity = differenceQuantity;
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
