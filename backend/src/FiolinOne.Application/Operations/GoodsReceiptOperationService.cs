using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.Operations;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.Operations;

public sealed class GoodsReceiptOperationService(IApplicationDbContext dbContext) : IGoodsReceiptOperationService
{
    public Task<GoodsReceiptVariantDto?> FindVariantByBarcodeAsync(string barcode, CancellationToken cancellationToken)
    {
        var trimmedBarcode = barcode.Trim();

        if (string.IsNullOrWhiteSpace(trimmedBarcode))
        {
            return Task.FromResult<GoodsReceiptVariantDto?>(null);
        }

        return ProjectVariant(QueryVariantEntities().Where(variant => variant.Barcode == trimmedBarcode))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<GoodsReceiptVariantDto?> GetVariantAsync(Guid productVariantId, CancellationToken cancellationToken)
    {
        return ProjectVariant(QueryVariantEntities().Where(variant => variant.Id == productVariantId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<GoodsReceiptOperationResultDto> CreateAsync(
        CreateGoodsReceiptOperationRequest request,
        CancellationToken cancellationToken)
    {
        var supplierExists = await dbContext.Suppliers.AnyAsync(
            supplier => supplier.Id == request.SupplierId && supplier.Active,
            cancellationToken);

        if (!supplierExists)
        {
            throw new InvalidOperationException("Tedarikçi bulunamadı veya aktif değil.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var variant = await dbContext.ProductVariants
            .FirstOrDefaultAsync(item => item.Id == request.ProductVariantId, cancellationToken);

        if (variant is null)
        {
            throw new InvalidOperationException("Ürün varyantı bulunamadı.");
        }

        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("Gelen adet 0'dan büyük olmalıdır.");
        }

        if (request.PurchasePrice < 0)
        {
            throw new InvalidOperationException("Alış fiyatı negatif olamaz.");
        }

        var stockBefore = variant.Stock;
        variant.RegisterGoodsReceipt(
            request.Quantity,
            request.PurchasePrice,
            request.SupplierId,
            request.Shelf,
            request.Box);

        var operation = new GoodsReceiptOperation(
            request.SupplierId,
            variant.Id,
            ToUtc(request.TransactionDate),
            NormalizeOptional(request.Description),
            request.PurchasePrice,
            request.Quantity,
            NormalizeOptional(request.Shelf),
            NormalizeOptional(request.Box),
            stockBefore,
            variant.Stock);

        await dbContext.GoodsReceiptOperations.AddAsync(operation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new GoodsReceiptOperationResultDto(
            operation.Id,
            variant.Id,
            variant.Barcode,
            operation.Quantity,
            operation.StockBefore,
            operation.StockAfter,
            operation.PurchasePrice,
            operation.Shelf,
            operation.Box,
            operation.TransactionDate);
    }

    private IQueryable<FiolinOne.Domain.Products.ProductVariant> QueryVariantEntities()
    {
        return dbContext.ProductVariants
            .AsNoTracking();
    }

    private static IQueryable<GoodsReceiptVariantDto> ProjectVariant(IQueryable<FiolinOne.Domain.Products.ProductVariant> source)
    {
        return source.Select(variant => new GoodsReceiptVariantDto(
                variant.ProductId,
                variant.Id,
                variant.Product!.ModelCode,
                variant.Product.ProductName,
                variant.ColorId,
                variant.Color!.Name,
                variant.SizeId,
                variant.Size!.Name,
                variant.Barcode,
                variant.Stock,
                variant.PurchasePrice,
                variant.Shelf,
                variant.Box,
                variant.LastSupplierId,
                variant.LastSupplier == null ? null : variant.LastSupplier.SupplierName));
    }

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
