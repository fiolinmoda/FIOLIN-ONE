namespace FiolinOne.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string ProductCode,
    string ProductName,
    string? Brand,
    string Category,
    string? Season,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
