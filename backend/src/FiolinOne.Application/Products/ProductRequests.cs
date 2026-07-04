namespace FiolinOne.Application.Products;

public sealed record CreateProductRequest(
    string ProductCode,
    string ProductName,
    string? Brand,
    string Category,
    string? Season,
    string Status);

public sealed record UpdateProductRequest(
    string ProductCode,
    string ProductName,
    string? Brand,
    string Category,
    string? Season,
    string Status);
