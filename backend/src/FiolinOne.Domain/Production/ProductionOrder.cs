using FiolinOne.Domain.Common;
using FiolinOne.Domain.Products;

namespace FiolinOne.Domain.Production;

public sealed class ProductionOrder : AuditableEntity
{
    private readonly List<ProductionOrderItem> items = [];
    private readonly List<ProductionTimelineEntry> timelineEntries = [];

    private ProductionOrder()
    {
    }

    public ProductionOrder(
        string productionNumber,
        Guid productId,
        int plannedQuantity,
        string productionReason,
        string? notes,
        string status)
    {
        ProductionNumber = productionNumber;
        ProductId = productId;
        PlannedQuantity = plannedQuantity;
        ProductionReason = productionReason;
        Notes = notes;
        Status = status;
    }

    public string ProductionNumber { get; private set; } = string.Empty;
    public Guid ProductId { get; private set; }
    public int PlannedQuantity { get; private set; }
    public string ProductionReason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public string Status { get; private set; } = ProductionStatuses.Planned;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Product? Product { get; private set; }
    public IReadOnlyCollection<ProductionOrderItem> Items => items;
    public IReadOnlyCollection<ProductionTimelineEntry> TimelineEntries => timelineEntries;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(string productionNumber, Guid productId, int plannedQuantity, string productionReason, string? notes, string status)
    {
        ProductionNumber = productionNumber;
        ProductId = productId;
        PlannedQuantity = plannedQuantity;
        ProductionReason = productionReason;
        Notes = notes;
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetStatus(string status)
    {
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

public static class ProductionStatuses
{
    public const string Planned = "PLANNED";
    public const string FabricAllocated = "FABRIC_ALLOCATED";
    public const string Cutting = "CUTTING";
    public const string AtWorkshop = "AT_WORKSHOP";
    public const string AtIroningPackaging = "AT_IRONING_PACKAGING";
    public const string ReadyForWarehouse = "READY_FOR_WAREHOUSE";
    public const string Completed = "COMPLETED";
    public const string Cancelled = "CANCELLED";
}
