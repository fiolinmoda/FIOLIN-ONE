using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Products;

public sealed class Product : Entity
{
    private Product()
    {
    }

    public Product(
        string productCode,
        string productName,
        string? brand,
        string category,
        string? season,
        string status)
    {
        ProductCode = productCode;
        ProductName = productName;
        Brand = brand;
        Category = category;
        Season = season;
        Status = status;
    }

    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public string? Brand { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string? Season { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string productCode,
        string productName,
        string? brand,
        string category,
        string? season,
        string status)
    {
        ProductCode = productCode;
        ProductName = productName;
        Brand = brand;
        Category = category;
        Season = season;
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
