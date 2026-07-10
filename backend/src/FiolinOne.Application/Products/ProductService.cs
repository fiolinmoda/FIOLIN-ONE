using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Application.MasterData;
using FiolinOne.Domain.Products;

namespace FiolinOne.Application.Products;

public sealed class ProductService(
    IProductRepository productRepository,
    IMasterDataRepository masterDataRepository,
    IDocumentNumberGenerator documentNumberGenerator) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? search, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(search, cancellationToken);

        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto?> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        return product is null ? null : ToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var productCode = await GetDocumentNumberAsync(request.ProductCode, DocumentNumberTypes.Product, cancellationToken);
        await EnsureProductCodeIsUniqueAsync(productCode, null, cancellationToken);
        await EnsureMasterDataExistsAsync(request.BrandId, "brands", cancellationToken);
        await EnsureMasterDataExistsAsync(request.CategoryId, "categories", cancellationToken);
        await EnsureMasterDataExistsAsync(request.SeasonId, "seasons", cancellationToken);

        var product = new Product(
            productCode,
            request.ProductName.Trim(),
            request.BrandId,
            request.CategoryId,
            request.SeasonId,
            request.Status.Trim());

        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);

        return ToDto(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        await EnsureProductCodeIsUniqueAsync(request.ProductCode, id, cancellationToken);
        await EnsureMasterDataExistsAsync(request.BrandId, "brands", cancellationToken);
        await EnsureMasterDataExistsAsync(request.CategoryId, "categories", cancellationToken);
        await EnsureMasterDataExistsAsync(request.SeasonId, "seasons", cancellationToken);

        product.Update(
            request.ProductCode.Trim(),
            request.ProductName.Trim(),
            request.BrandId,
            request.CategoryId,
            request.SeasonId,
            request.Status.Trim());

        await productRepository.SaveChangesAsync(cancellationToken);

        return ToDto(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        productRepository.Delete(product);
        await productRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureProductCodeIsUniqueAsync(
        string productCode,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await productRepository.ExistsByCodeAsync(productCode.Trim(), excludedId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Bu ürün kodu zaten kullanılıyor.");
        }
    }

    private async Task EnsureMasterDataExistsAsync(Guid? id, string type, CancellationToken cancellationToken)
    {
        if (!id.HasValue)
        {
            return;
        }

        if (!await masterDataRepository.ExistsAsync(type, id.Value, cancellationToken))
        {
            throw new InvalidOperationException("Seçilen tanım kaydı bulunamadı.");
        }
    }

    private async Task<string> GetDocumentNumberAsync(string? requestedNumber, string documentType, CancellationToken cancellationToken)
    {
        return string.IsNullOrWhiteSpace(requestedNumber)
            ? await documentNumberGenerator.GenerateAsync(documentType, cancellationToken)
            : requestedNumber.Trim();
    }

    private static ProductDto ToDto(Product product)
    {
        var variants = product.Variants
            .OrderBy(variant => variant.Color?.Name)
            .ThenBy(variant => variant.Size?.SortOrder)
            .ThenBy(variant => variant.Size?.Name)
            .ToList();

        var colorGroups = variants
            .GroupBy(variant => new { variant.ColorId, Color = variant.Color?.Name ?? string.Empty })
            .OrderBy(group => group.Key.Color)
            .Select(group => new ProductColorGroupDto(
                group.Key.ColorId,
                group.Key.Color,
                group.Sum(variant => variant.Stock),
                group
                    .OrderBy(variant => variant.Size?.SortOrder)
                    .ThenBy(variant => variant.Size?.Name)
                    .Select(variant => new ProductSizeVariantDto(
                        variant.Id,
                        variant.SizeId,
                        variant.Size?.Name ?? string.Empty,
                        variant.Barcode,
                        variant.Stock,
                        variant.PurchasePrice,
                        variant.SalesPrice))
                    .ToList()))
            .ToList();

        return new ProductDto(
            product.Id,
            product.ProductCode,
            product.ProductName,
            product.BrandId,
            product.Brand?.Name,
            product.CategoryId,
            product.Category?.Name,
            product.SeasonId,
            product.Season?.Name,
            product.Status,
            product.ImageUrl,
            variants.Select(variant => variant.ColorId).Distinct().Count(),
            variants.Select(variant => variant.SizeId).Distinct().Count(),
            variants.Count,
            variants.Sum(variant => variant.Stock),
            colorGroups,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
