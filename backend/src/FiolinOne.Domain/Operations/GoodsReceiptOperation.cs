using FiolinOne.Domain.Common;
using FiolinOne.Domain.Products;
using FiolinOne.Domain.Purchasing;

namespace FiolinOne.Domain.Operations;

public sealed class GoodsReceiptOperation : Entity
{
    private GoodsReceiptOperation()
    {
    }

    public GoodsReceiptOperation(
        Guid supplierId,
        Guid productVariantId,
        DateTime transactionDate,
        string? description,
        decimal purchasePrice,
        int quantity,
        string? shelf,
        string? box,
        int stockBefore,
        int stockAfter)
    {
        SupplierId = supplierId;
        ProductVariantId = productVariantId;
        TransactionDate = transactionDate;
        Description = description;
        PurchasePrice = purchasePrice;
        Quantity = quantity;
        Shelf = shelf;
        Box = box;
        StockBefore = stockBefore;
        StockAfter = stockAfter;
    }

    public Guid SupplierId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string MovementType { get; private set; } = GoodsReceiptMovementTypes.GoodsReceipt;
    public string? Description { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public int Quantity { get; private set; }
    public string? Shelf { get; private set; }
    public string? Box { get; private set; }
    public int StockBefore { get; private set; }
    public int StockAfter { get; private set; }
    public Supplier? Supplier { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
}

public static class GoodsReceiptMovementTypes
{
    public const string GoodsReceipt = "MAL_KABUL";
}
