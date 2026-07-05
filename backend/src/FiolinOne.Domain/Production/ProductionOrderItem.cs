using FiolinOne.Domain.Common;
using FiolinOne.Domain.Products;

namespace FiolinOne.Domain.Production;

public sealed class ProductionOrderItem : AuditableEntity
{
    private ProductionOrderItem()
    {
    }

    public ProductionOrderItem(Guid productionOrderId, Guid productVariantId, int plannedQuantity)
    {
        ProductionOrderId = productionOrderId;
        ProductVariantId = productVariantId;
        PlannedQuantity = plannedQuantity;
    }

    public Guid ProductionOrderId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public int PlannedQuantity { get; private set; }
    public bool BarcodeGenerated { get; private set; }
    public bool BarcodePrinted { get; private set; }
    public string? BarcodeValue { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public ProductionOrder? ProductionOrder { get; private set; }
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
