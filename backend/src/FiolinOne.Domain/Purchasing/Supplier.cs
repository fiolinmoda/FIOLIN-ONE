using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Purchasing;

public sealed class Supplier : AuditableEntity
{
    private Supplier()
    {
    }

    public Supplier(
        string supplierCode,
        string supplierName,
        string? phone,
        string? email,
        string? address,
        string? taxNumber,
        string? paymentTerm,
        bool active)
    {
        SupplierCode = supplierCode;
        SupplierName = supplierName;
        Phone = phone;
        Email = email;
        Address = address;
        TaxNumber = taxNumber;
        PaymentTerm = paymentTerm;
        Active = active;
    }

    public string SupplierCode { get; private set; } = string.Empty;
    public string SupplierName { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? PaymentTerm { get; private set; }
    public bool Active { get; private set; } = true;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string supplierCode,
        string supplierName,
        string? phone,
        string? email,
        string? address,
        string? taxNumber,
        string? paymentTerm,
        bool active)
    {
        SupplierCode = supplierCode;
        SupplierName = supplierName;
        Phone = phone;
        Email = email;
        Address = address;
        TaxNumber = taxNumber;
        PaymentTerm = paymentTerm;
        Active = active;
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
