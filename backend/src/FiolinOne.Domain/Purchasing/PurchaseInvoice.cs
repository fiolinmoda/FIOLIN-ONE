using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Purchasing;

public sealed class PurchaseInvoice : AuditableEntity
{
    private readonly List<PurchaseInvoiceItem> items = [];

    private PurchaseInvoice()
    {
    }

    public PurchaseInvoice(
        string invoiceNumber,
        DateTime invoiceDate,
        Guid supplierId,
        Guid? purchaseOrderId,
        decimal invoiceAmount,
        string status,
        string? notes)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        SupplierId = supplierId;
        PurchaseOrderId = purchaseOrderId;
        InvoiceAmount = invoiceAmount;
        Status = status;
        Notes = notes;
    }

    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateTime InvoiceDate { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? PurchaseOrderId { get; private set; }
    public decimal InvoiceAmount { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Supplier? Supplier { get; private set; }
    public PurchaseOrder? PurchaseOrder { get; private set; }
    public IReadOnlyCollection<PurchaseInvoiceItem> Items => items;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string invoiceNumber,
        DateTime invoiceDate,
        Guid supplierId,
        Guid? purchaseOrderId,
        decimal invoiceAmount,
        string status,
        string? notes)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        SupplierId = supplierId;
        PurchaseOrderId = purchaseOrderId;
        InvoiceAmount = invoiceAmount;
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
