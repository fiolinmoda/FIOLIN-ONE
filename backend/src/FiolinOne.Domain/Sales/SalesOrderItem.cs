using FiolinOne.Domain.Common;
using FiolinOne.Domain.Products;

namespace FiolinOne.Domain.Sales;

public sealed class SalesOrderItem : AuditableEntity
{
    private SalesOrderItem()
    {
    }

    public SalesOrderItem(
        Guid salesOrderId,
        ProductVariant productVariant,
        int quantity,
        decimal unitPrice)
    {
        SalesOrderId = salesOrderId;
        ProductVariantId = productVariant.Id;
        ProductVariant = productVariant;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalAmount = quantity * unitPrice;
    }

    public Guid SalesOrderId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public SalesOrder? SalesOrder { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
