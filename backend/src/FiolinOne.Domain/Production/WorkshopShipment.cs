using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Production;

public sealed class WorkshopShipment : AuditableEntity
{
    private WorkshopShipment()
    {
    }

    public WorkshopShipment(Guid productionOrderId, string workshop, DateTime shipmentDate, DateTime? expectedReturnDate, int sentQuantity, string? notes, string status)
    {
        ProductionOrderId = productionOrderId;
        Workshop = workshop;
        ShipmentDate = shipmentDate;
        ExpectedReturnDate = expectedReturnDate;
        SentQuantity = sentQuantity;
        Notes = notes;
        Status = status;
    }

    public Guid ProductionOrderId { get; private set; }
    public string Workshop { get; private set; } = string.Empty;
    public DateTime ShipmentDate { get; private set; }
    public DateTime? ExpectedReturnDate { get; private set; }
    public int SentQuantity { get; private set; }
    public string? Notes { get; private set; }
    public string Status { get; private set; } = WorkshopShipmentStatuses.Sent;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public ProductionOrder? ProductionOrder { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void SetStatus(string status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public static class WorkshopShipmentStatuses
{
    public const string Sent = "SENT";
    public const string PartialReturn = "PARTIAL_RETURN";
    public const string Returned = "RETURNED";
}
