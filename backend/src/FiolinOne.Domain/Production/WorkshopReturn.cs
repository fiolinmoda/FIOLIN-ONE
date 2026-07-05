using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Production;

public sealed class WorkshopReturn : AuditableEntity
{
    private WorkshopReturn()
    {
    }

    public WorkshopReturn(Guid productionOrderId, Guid? workshopShipmentId, int returnedQuantity, int extraQuantity, int missingQuantity, DateTime returnDate, string? notes)
    {
        ProductionOrderId = productionOrderId;
        WorkshopShipmentId = workshopShipmentId;
        ReturnedQuantity = returnedQuantity;
        ExtraQuantity = extraQuantity;
        MissingQuantity = missingQuantity;
        ReturnDate = returnDate;
        Notes = notes;
    }

    public Guid ProductionOrderId { get; private set; }
    public Guid? WorkshopShipmentId { get; private set; }
    public int ReturnedQuantity { get; private set; }
    public int ExtraQuantity { get; private set; }
    public int MissingQuantity { get; private set; }
    public DateTime ReturnDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public ProductionOrder? ProductionOrder { get; private set; }
    public WorkshopShipment? WorkshopShipment { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;
}
