using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Purchasing;

public sealed class PurchaseOrder : AuditableEntity
{
    private readonly List<PurchaseOrderItem> items = [];

    private PurchaseOrder()
    {
    }

    public PurchaseOrder(
        string purchaseNumber,
        Guid supplierId,
        DateTime orderDate,
        DateTime? expectedDate,
        string status,
        string? notes)
    {
        PurchaseNumber = purchaseNumber;
        SupplierId = supplierId;
        OrderDate = orderDate;
        ExpectedDate = expectedDate;
        Status = status;
        Notes = notes;
    }

    public string PurchaseNumber { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Supplier? Supplier { get; private set; }
    public IReadOnlyCollection<PurchaseOrderItem> Items => items;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string purchaseNumber,
        Guid supplierId,
        DateTime orderDate,
        DateTime? expectedDate,
        string status,
        string? notes)
    {
        PurchaseNumber = purchaseNumber;
        SupplierId = supplierId;
        OrderDate = orderDate;
        ExpectedDate = expectedDate;
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
