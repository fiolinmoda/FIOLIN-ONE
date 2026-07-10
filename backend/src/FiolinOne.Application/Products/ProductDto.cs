namespace FiolinOne.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string ProductCode,
    string ProductName,
    Guid? BrandId,
    string? Brand,
    Guid? CategoryId,
    string? Category,
    Guid? SeasonId,
    string? Season,
    string Status,
    string? ImageUrl,
    int ColorCount,
    int SizeCount,
    int VariantCount,
    int TotalStock,
    IReadOnlyList<ProductColorGroupDto> ColorGroups,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ProductColorGroupDto(
    Guid ColorId,
    string Color,
    int TotalStock,
    IReadOnlyList<ProductSizeVariantDto> Sizes);

public sealed record ProductSizeVariantDto(
    Guid VariantId,
    Guid SizeId,
    string Size,
    string Barcode,
    int Stock,
    decimal PurchasePrice,
    decimal SalesPrice);
