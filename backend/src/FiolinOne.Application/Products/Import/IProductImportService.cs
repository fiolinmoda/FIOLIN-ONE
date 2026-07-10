namespace FiolinOne.Application.Products.Import;

public interface IProductImportService
{
    Task<ProductImportPreviewDto> PreviewAsync(
        Stream fileStream,
        string fileName,
        ProductImportPreviewRequest request,
        CancellationToken cancellationToken);

    Task<ProductImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        string userName,
        ProductImportExecuteRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductImportHistoryDto>> GetHistoryAsync(CancellationToken cancellationToken);
}
