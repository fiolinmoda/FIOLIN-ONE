using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Production;

public sealed class ProductionTimelineEntry : AuditableEntity
{
    private ProductionTimelineEntry()
    {
    }

    public ProductionTimelineEntry(Guid productionOrderId, string eventType, string description, DateTime eventDate)
    {
        ProductionOrderId = productionOrderId;
        EventType = eventType;
        Description = description;
        EventDate = eventDate;
    }

    public Guid ProductionOrderId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime EventDate { get; private set; }
    public ProductionOrder? ProductionOrder { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
}
