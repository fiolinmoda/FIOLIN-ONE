namespace FiolinOne.Application.Products.Variants;

public sealed record CreateProductVariantRequest(
    Guid ColorId,
    Guid SizeId,
    string Barcode,
    string? TrendyolSku,
    int Stock,
    string Status,
    decimal? PurchasePrice = null,
    decimal? SalesPrice = null);

public sealed record UpdateProductVariantRequest(
    Guid ColorId,
    Guid SizeId,
    string Barcode,
    string? TrendyolSku,
    int Stock,
    string Status,
    decimal? PurchasePrice = null,
    decimal? SalesPrice = null);
