namespace FiolinOne.Application.Products;

public sealed record CreateProductRequest(
    string ModelCode,
    string ProductCode,
    string ProductName,
    Guid? BrandId,
    Guid? CategoryId,
    Guid? SeasonId,
    string Status);

public sealed record UpdateProductRequest(
    string ModelCode,
    string ProductCode,
    string ProductName,
    Guid? BrandId,
    Guid? CategoryId,
    Guid? SeasonId,
    string Status);
