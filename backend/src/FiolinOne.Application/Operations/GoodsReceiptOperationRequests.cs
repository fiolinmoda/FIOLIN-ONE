namespace FiolinOne.Application.Operations;

public sealed record CreateGoodsReceiptOperationRequest(
    Guid SupplierId,
    Guid ProductVariantId,
    DateTime TransactionDate,
    string? Description,
    decimal PurchasePrice,
    int Quantity,
    string? Shelf,
    string? Box);
