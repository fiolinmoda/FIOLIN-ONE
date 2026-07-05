using FiolinOne.Domain.Common;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Purchasing;

namespace FiolinOne.Domain.Fabric;

public sealed class Fabric : AuditableEntity
{
    private Fabric()
    {
    }

    public Fabric(
        string fabricCode,
        string fabricName,
        Guid supplierId,
        Guid colorId,
        string? composition,
        decimal width,
        decimal weightGsm,
        string unit,
        decimal purchasePrice,
        decimal currentStockKg,
        decimal minimumStock,
        string status,
        string? notes)
    {
        FabricCode = fabricCode;
        FabricName = fabricName;
        SupplierId = supplierId;
        ColorId = colorId;
        Composition = composition;
        Width = width;
        WeightGsm = weightGsm;
        Unit = unit;
        PurchasePrice = purchasePrice;
        CurrentStockKg = currentStockKg;
        MinimumStock = minimumStock;
        Status = currentStockKg <= 0 ? FabricStatuses.OutOfStock : status;
        Notes = notes;
    }

    public string FabricCode { get; private set; } = string.Empty;
    public string FabricName { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public Guid ColorId { get; private set; }
    public string? Composition { get; private set; }
    public decimal Width { get; private set; }
    public decimal WeightGsm { get; private set; }
    public string Unit { get; private set; } = "Kg";
    public decimal PurchasePrice { get; private set; }
    public decimal CurrentStockKg { get; private set; }
    public decimal MinimumStock { get; private set; }
    public string Status { get; private set; } = FabricStatuses.Active;
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public Supplier? Supplier { get; private set; }
    public Color? Color { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string fabricCode,
        string fabricName,
        Guid supplierId,
        Guid colorId,
        string? composition,
        decimal width,
        decimal weightGsm,
        string unit,
        decimal purchasePrice,
        decimal minimumStock,
        string status,
        string? notes)
    {
        FabricCode = fabricCode;
        FabricName = fabricName;
        SupplierId = supplierId;
        ColorId = colorId;
        Composition = composition;
        Width = width;
        WeightGsm = weightGsm;
        Unit = unit;
        PurchasePrice = purchasePrice;
        MinimumStock = minimumStock;
        Status = CurrentStockKg <= 0 ? FabricStatuses.OutOfStock : status;
        Notes = notes;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ApplyStockChange(decimal quantityDelta)
    {
        var nextStock = CurrentStockKg + quantityDelta;

        if (nextStock < 0)
        {
            throw new InvalidOperationException("Fabric stock cannot be negative.");
        }

        CurrentStockKg = nextStock;
        Status = CurrentStockKg <= 0 ? FabricStatuses.OutOfStock : FabricStatuses.Active;
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

public static class FabricStatuses
{
    public const string Active = "Active";
    public const string OutOfStock = "OUT OF STOCK";
}
