using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Production;

public sealed class WarehouseEntry : AuditableEntity
{
    private WarehouseEntry()
    {
    }

    public WarehouseEntry(Guid productionOrderId, int actualQuantity, DateTime warehouseDate, string? notes)
    {
        ProductionOrderId = productionOrderId;
        ActualQuantity = actualQuantity;
        WarehouseDate = warehouseDate;
        Notes = notes;
    }

    public Guid ProductionOrderId { get; private set; }
    public int ActualQuantity { get; private set; }
    public DateTime WarehouseDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public ProductionOrder? ProductionOrder { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;
}
