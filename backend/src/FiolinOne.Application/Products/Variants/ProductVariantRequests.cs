namespace FiolinOne.Application.Products.Variants;

public sealed record CreateProductVariantRequest(
    string Color,
    string Size,
    string Barcode,
    string? TrendyolSku,
    int Stock,
    string Status);

public sealed record UpdateProductVariantRequest(
    string Color,
    string Size,
    string Barcode,
    string? TrendyolSku,
    int Stock,
    string Status);
