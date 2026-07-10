using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Application.MasterData;
using FiolinOne.Domain.Products;
using System.Text.RegularExpressions;

namespace FiolinOne.Application.Products;

public sealed class ProductService(
    IProductRepository productRepository,
    IMasterDataRepository masterDataRepository,
    IDocumentNumberGenerator documentNumberGenerator) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? search, CancellationToken cancellationToken)
    {
        var rows = await productRepository.GetListRowsAsync(cancellationToken);

        return BuildModelCards(rows, search);
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

    private static IReadOnlyList<ProductDto> BuildModelCards(
        IReadOnlyList<ProductListRowDto> rows,
        string? search)
    {
        var groups = rows
            .GroupBy(row => new ModelGroupKey(
                InferModelCode(row.ProductCode),
                row.BrandId,
                row.CategoryId,
                row.SeasonId))
            .Where(group => MatchesSearch(group, search))
            .OrderBy(group => group.Key.ModelCode, StringComparer.CurrentCultureIgnoreCase)
            .Select(BuildModelCard)
            .ToList();

        return groups;
    }

    private static ProductDto BuildModelCard(IGrouping<ModelGroupKey, ProductListRowDto> group)
    {
        var representative = group
            .OrderByDescending(row => !string.IsNullOrWhiteSpace(row.ImageUrl))
            .ThenBy(row => row.ProductCode.Length)
            .ThenBy(row => row.ProductCode)
            .First();

        var variantRows = group
            .Where(row => row.VariantId.HasValue && row.ColorId.HasValue && row.SizeId.HasValue)
            .ToList();

        var colorGroups = variantRows
            .GroupBy(row => new { ColorId = row.ColorId!.Value, Color = row.Color ?? string.Empty })
            .OrderBy(colorGroup => colorGroup.Key.Color, StringComparer.CurrentCultureIgnoreCase)
            .Select(colorGroup => new ProductColorGroupDto(
                colorGroup.Key.ColorId,
                colorGroup.Key.Color,
                colorGroup.Sum(row => row.Stock),
                colorGroup
                    .OrderBy(row => row.SizeSortOrder ?? int.MaxValue)
                    .ThenBy(row => row.Size, StringComparer.CurrentCultureIgnoreCase)
                    .Select(row => new ProductSizeVariantDto(
                        row.VariantId!.Value,
                        row.SizeId!.Value,
                        row.Size ?? string.Empty,
                        row.Barcode ?? string.Empty,
                        row.Stock,
                        row.PurchasePrice,
                        row.SalesPrice))
                    .ToList()))
            .ToList();

        return new ProductDto(
            representative.ProductId,
            group.Key.ModelCode,
            representative.ProductName,
            representative.BrandId,
            representative.Brand,
            representative.CategoryId,
            representative.Category,
            representative.SeasonId,
            representative.Season,
            representative.Status,
            representative.ImageUrl,
            variantRows
                .Where(row => row.ColorId.HasValue)
                .Select(row => row.ColorId!.Value)
                .Distinct()
                .Count(),
            variantRows
                .Where(row => row.SizeId.HasValue)
                .Select(row => row.SizeId!.Value)
                .Distinct()
                .Count(),
            variantRows.Count,
            variantRows.Sum(row => row.Stock),
            colorGroups,
            group.Min(row => row.CreatedAt),
            group.Max(row => row.UpdatedAt));
    }

    private static bool MatchesSearch(IGrouping<ModelGroupKey, ProductListRowDto> group, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }

        var term = search.Trim();

        return Contains(group.Key.ModelCode, term) ||
            group.Any(row =>
                Contains(row.ProductCode, term) ||
                Contains(row.ProductName, term) ||
                Contains(row.Brand, term) ||
                Contains(row.Category, term) ||
                Contains(row.Season, term) ||
                Contains(row.Color, term) ||
                Contains(row.Size, term));
    }

    private static bool Contains(string? value, string term)
    {
        return value?.Contains(term, StringComparison.CurrentCultureIgnoreCase) == true;
    }

    private static string InferModelCode(string productCode)
    {
        var code = productCode.Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            return code;
        }

        var compactCode = Regex.Replace(code, @"[\s_\-]+", string.Empty);
        var leadingDigits = Regex.Match(compactCode, @"^\d{3,6}(?=[A-Za-z])");

        if (leadingDigits.Success)
        {
            return leadingDigits.Value;
        }

        var numericSize = Regex.Match(compactCode, @"^(?<model>.+?[A-Za-z])(?<size>\d{2,3}(?:[,.]0)?)$", RegexOptions.IgnoreCase);

        if (numericSize.Success && numericSize.Groups["model"].Value.Length >= 3)
        {
            return numericSize.Groups["model"].Value;
        }

        var alphaSize = Regex.Match(compactCode, @"^(?<model>.+\d.*?)(?<size>XXS|XS|S|M|L|XL|XXL|2XL|3XL|4XL|5XL)$", RegexOptions.IgnoreCase);

        if (alphaSize.Success && alphaSize.Groups["model"].Value.Length >= 3)
        {
            return alphaSize.Groups["model"].Value;
        }

        return compactCode;
    }

    private sealed record ModelGroupKey(
        string ModelCode,
        Guid? BrandId,
        Guid? CategoryId,
        Guid? SeasonId);
}
