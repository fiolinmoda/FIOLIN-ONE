namespace FiolinOne.Application.Operations;

public sealed record GoodsReceiptVariantDto(
    Guid ProductId,
    Guid ProductVariantId,
    string ModelCode,
    string ProductName,
    Guid ColorId,
    string Color,
    Guid SizeId,
    string Size,
    string Barcode,
    int Stock,
    decimal LastPurchasePrice,
    string? Shelf,
    string? Box,
    Guid? LastSupplierId,
    string? LastSupplierName);

public sealed record GoodsReceiptOperationResultDto(
    Guid Id,
    Guid ProductVariantId,
    string Barcode,
    int Quantity,
    int StockBefore,
    int StockAfter,
    decimal PurchasePrice,
    string? Shelf,
    string? Box,
    DateTime TransactionDate);
