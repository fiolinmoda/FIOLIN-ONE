using FiolinOne.Domain.Common;
using FiolinOne.Domain.MasterData;

namespace FiolinOne.Domain.Products;

public sealed class Product : Entity
{
    private Product()
    {
    }

    public Product(
        string productCode,
        string productName,
        Guid? brandId,
        Guid? categoryId,
        Guid? seasonId,
        string status,
        string? imageUrl = null)
    {
        ProductCode = productCode;
        ProductName = productName;
        BrandId = brandId;
        CategoryId = categoryId;
        SeasonId = seasonId;
        Status = status;
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
    }

    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public Guid? BrandId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? SeasonId { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public Brand? Brand { get; private set; }
    public Category? Category { get; private set; }
    public Season? Season { get; private set; }
    public IReadOnlyCollection<ProductVariant> Variants => variants;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    private readonly List<ProductVariant> variants = [];

    public void Update(
        string productCode,
        string productName,
        Guid? brandId,
        Guid? categoryId,
        Guid? seasonId,
        string status,
        string? imageUrl = null)
    {
        ProductCode = productCode;
        ProductName = productName;
        BrandId = brandId;
        CategoryId = categoryId;
        SeasonId = seasonId;
        Status = status;
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
