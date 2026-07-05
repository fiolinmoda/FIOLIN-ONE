using FiolinOne.Domain.Common;
using FiolinOne.Domain.Fabric;

namespace FiolinOne.Domain.Production;

public sealed class CuttingRecord : AuditableEntity
{
    private CuttingRecord()
    {
    }

    public CuttingRecord(Guid productionOrderId, Guid fabricId, decimal consumedWeightKg, decimal wasteWeightKg, DateTime cuttingDate, string? operatorName, string? notes)
    {
        ProductionOrderId = productionOrderId;
        FabricId = fabricId;
        ConsumedWeightKg = consumedWeightKg;
        WasteWeightKg = wasteWeightKg;
        CuttingDate = cuttingDate;
        OperatorName = operatorName;
        Notes = notes;
    }

    public Guid ProductionOrderId { get; private set; }
    public Guid FabricId { get; private set; }
    public decimal ConsumedWeightKg { get; private set; }
    public decimal WasteWeightKg { get; private set; }
    public DateTime CuttingDate { get; private set; }
    public string? OperatorName { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public ProductionOrder? ProductionOrder { get; private set; }
    public Fabric.Fabric? Fabric { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;
}
