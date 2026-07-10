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
    DateTime CreatedAt,
    DateTime? UpdatedAt);
