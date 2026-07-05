using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Purchasing;

public sealed class GoodsReceipt : AuditableEntity
{
    private readonly List<GoodsReceiptItem> items = [];

    private GoodsReceipt()
    {
    }

    public GoodsReceipt(
        string receiptNumber,
        Guid supplierId,
        Guid? purchaseOrderId,
        DateTime receiptDate,
        string warehouse,
        string status,
        string? notes)
    {
        ReceiptNumber = receiptNumber;
        SupplierId = supplierId;
        PurchaseOrderId = purchaseOrderId;
        ReceiptDate = receiptDate;
        Warehouse = warehouse;
        Status = status;
        Notes = notes;
    }

    public string ReceiptNumber { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public Guid? PurchaseOrderId { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public string Warehouse { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Supplier? Supplier { get; private set; }
    public PurchaseOrder? PurchaseOrder { get; private set; }
    public IReadOnlyCollection<GoodsReceiptItem> Items => items;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string receiptNumber,
        Guid supplierId,
        Guid? purchaseOrderId,
        DateTime receiptDate,
        string warehouse,
        string status,
        string? notes)
    {
        ReceiptNumber = receiptNumber;
        SupplierId = supplierId;
        PurchaseOrderId = purchaseOrderId;
        ReceiptDate = receiptDate;
        Warehouse = warehouse;
        Status = status;
        Notes = notes;
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
