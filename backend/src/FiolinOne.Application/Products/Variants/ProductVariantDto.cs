namespace FiolinOne.Application.Products.Variants;

public sealed record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    Guid ColorId,
    string Color,
    Guid SizeId,
    string Size,
    string Barcode,
    string? TrendyolSku,
    int Stock,
    decimal PurchasePrice,
    decimal SalesPrice,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
