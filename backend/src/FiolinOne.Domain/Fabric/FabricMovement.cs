using FiolinOne.Domain.Common;
using FiolinOne.Domain.Purchasing;

namespace FiolinOne.Domain.Fabric;

public sealed class FabricMovement : AuditableEntity
{
    private FabricMovement()
    {
    }

    public FabricMovement(
        Guid fabricId,
        string movementType,
        decimal quantityKg,
        decimal unitPrice,
        Guid? supplierId,
        Guid? purchaseOrderId,
        string? batchLot,
        string warehouse,
        DateTime movementDate,
        string? notes)
    {
        FabricId = fabricId;
        MovementType = movementType;
        QuantityKg = quantityKg;
        UnitPrice = unitPrice;
        SupplierId = supplierId;
        PurchaseOrderId = purchaseOrderId;
        BatchLot = batchLot;
        Warehouse = warehouse;
        MovementDate = movementDate;
        Notes = notes;
    }

    public Guid FabricId { get; private set; }
    public string MovementType { get; private set; } = string.Empty;
    public decimal QuantityKg { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Guid? SupplierId { get; private set; }
    public Guid? PurchaseOrderId { get; private set; }
    public string? BatchLot { get; private set; }
    public string Warehouse { get; private set; } = string.Empty;
    public DateTime MovementDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Fabric? Fabric { get; private set; }
    public Supplier? Supplier { get; private set; }
    public PurchaseOrder? PurchaseOrder { get; private set; }
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

public static class FabricMovementTypes
{
    public const string Purchase = "Purchase";
    public const string ProductionConsumption = "Production Consumption";
    public const string ManualAdjustment = "Manual Adjustment";
    public const string InventoryCount = "Inventory Count";
    public const string Return = "Return";
}
