using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Fabric;

public sealed class FabricReservation : AuditableEntity
{
    private FabricReservation()
    {
    }

    public FabricReservation(
        Guid fabricId,
        string reservationNumber,
        string productionReference,
        decimal reservedQuantityKg,
        DateTime reservationDate,
        string status,
        string? notes)
    {
        FabricId = fabricId;
        ReservationNumber = reservationNumber;
        ProductionReference = productionReference;
        ReservedQuantityKg = reservedQuantityKg;
        ReservationDate = reservationDate;
        Status = status;
        Notes = notes;
    }

    public Guid FabricId { get; private set; }
    public string ReservationNumber { get; private set; } = string.Empty;
    public string ProductionReference { get; private set; } = string.Empty;
    public decimal ReservedQuantityKg { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Fabric? Fabric { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        Guid fabricId,
        string reservationNumber,
        string productionReference,
        decimal reservedQuantityKg,
        DateTime reservationDate,
        string status,
        string? notes)
    {
        FabricId = fabricId;
        ReservationNumber = reservationNumber;
        ProductionReference = productionReference;
        ReservedQuantityKg = reservedQuantityKg;
        ReservationDate = reservationDate;
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
