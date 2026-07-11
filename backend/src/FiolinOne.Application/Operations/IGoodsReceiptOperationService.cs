namespace FiolinOne.Application.Operations;

public interface IGoodsReceiptOperationService
{
    Task<GoodsReceiptVariantDto?> FindVariantByBarcodeAsync(string barcode, CancellationToken cancellationToken);
    Task<GoodsReceiptVariantDto?> GetVariantAsync(Guid productVariantId, CancellationToken cancellationToken);
    Task<GoodsReceiptOperationResultDto> CreateAsync(CreateGoodsReceiptOperationRequest request, CancellationToken cancellationToken);
}
