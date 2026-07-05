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
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
