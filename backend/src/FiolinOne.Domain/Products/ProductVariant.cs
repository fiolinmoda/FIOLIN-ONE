using FiolinOne.Domain.Common;
using FiolinOne.Domain.MasterData;

namespace FiolinOne.Domain.Products;

public sealed class ProductVariant : Entity
{
    private ProductVariant()
    {
    }

    public ProductVariant(
        Guid productId,
        Guid colorId,
        Guid sizeId,
        string barcode,
        string? trendyolSku,
        int stock,
        string status,
        decimal purchasePrice = 0,
        decimal salesPrice = 0)
    {
        ProductId = productId;
        ColorId = colorId;
        SizeId = sizeId;
        Barcode = barcode;
        TrendyolSku = trendyolSku;
        Stock = stock;
        Status = status;
        PurchasePrice = purchasePrice;
        SalesPrice = salesPrice;
    }

    public Guid ProductId { get; private set; }
    public Guid ColorId { get; private set; }
    public Guid SizeId { get; private set; }
    public string Barcode { get; private set; } = string.Empty;
    public string? TrendyolSku { get; private set; }
    public int Stock { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public decimal SalesPrice { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public Product? Product { get; private set; }
    public Color? Color { get; private set; }
    public Size? Size { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        Guid colorId,
        Guid sizeId,
        string barcode,
        string? trendyolSku,
        int stock,
        string status,
        decimal purchasePrice = 0,
        decimal salesPrice = 0)
    {
        ColorId = colorId;
        SizeId = sizeId;
        Barcode = barcode;
        TrendyolSku = trendyolSku;
        Stock = stock;
        Status = status;
        PurchasePrice = purchasePrice;
        SalesPrice = salesPrice;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity < 0)
        {
            throw new InvalidOperationException("Stok artış miktarı negatif olamaz.");
        }

        Stock += quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity < 0)
        {
            throw new InvalidOperationException("Stok düşüş miktarı negatif olamaz.");
        }

        if (Stock - quantity < 0)
        {
            throw new InvalidOperationException("Satış miktarı kullanılabilir stok miktarını aşamaz.");
        }

        Stock -= quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
