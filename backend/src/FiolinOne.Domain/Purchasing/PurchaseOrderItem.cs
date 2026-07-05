using FiolinOne.Domain.Common;
using FiolinOne.Domain.MasterData;

namespace FiolinOne.Domain.Purchasing;

public sealed class PurchaseOrderItem : AuditableEntity
{
    private PurchaseOrderItem()
    {
    }

    public PurchaseOrderItem(
        Guid purchaseOrderId,
        Guid? fabricTypeId,
        Guid? colorId,
        string itemName,
        decimal quantity,
        string unit,
        decimal unitPrice,
        decimal receivedQuantity,
        string status)
    {
        PurchaseOrderId = purchaseOrderId;
        FabricTypeId = fabricTypeId;
        ColorId = colorId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        ReceivedQuantity = receivedQuantity;
        Status = status;
    }

    public Guid PurchaseOrderId { get; private set; }
    public Guid? FabricTypeId { get; private set; }
    public Guid? ColorId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal RemainingQuantity => Quantity - ReceivedQuantity;
    public string Status { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public PurchaseOrder? PurchaseOrder { get; private set; }
    public FabricType? FabricType { get; private set; }
    public Color? Color { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        Guid? fabricTypeId,
        Guid? colorId,
        string itemName,
        decimal quantity,
        string unit,
        decimal unitPrice,
        decimal receivedQuantity,
        string status)
    {
        FabricTypeId = fabricTypeId;
        ColorId = colorId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        ReceivedQuantity = receivedQuantity;
        Status = status;
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
