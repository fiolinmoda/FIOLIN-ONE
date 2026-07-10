namespace FiolinOne.Application.Products.Import;

public sealed record ProductImportMapping(
    string? ModelCode,
    string? ProductName,
    string? Brand,
    string? Category,
    string? Season,
    string? Color,
    string? Size,
    string? FabricType,
    string? PurchasePrice,
    string? SalesPrice,
    string? Stock);

public sealed record ProductImportPreviewRequest(
    ProductImportMapping? Mapping,
    string MissingMasterDataMode);

public sealed record ProductImportExecuteRequest(
    ProductImportMapping Mapping,
    string MissingMasterDataMode,
    string? ProfileName,
    bool SaveProfile);

public sealed record ProductImportPreviewDto(
    string FileName,
    string FileSignature,
    IReadOnlyList<string> Headers,
    ProductImportMapping SuggestedMapping,
    ProductImportProfileDto? SavedProfile,
    ProductImportSummaryDto Summary,
    IReadOnlyList<ProductImportPreviewRowDto> Rows);

public sealed record ProductImportSummaryDto(
    int Total,
    int Valid,
    int MissingField,
    int Error,
    int NewProducts,
    int ExistingProducts,
    int Skipped);

public sealed record ProductImportPreviewRowDto(
    int RowNumber,
    string ModelCode,
    string ProductName,
    string Status,
    IReadOnlyList<string> Errors);

public sealed record ProductImportResultDto(
    ProductImportSummaryDto Summary,
    int Inserted,
    int Existing,
    int Skipped,
    int Error,
    int DurationMilliseconds,
    IReadOnlyList<ProductImportErrorRowDto> ErrorRows);

public sealed record ProductImportErrorRowDto(
    int RowNumber,
    string ModelCode,
    string ProductName,
    string Reason);

public sealed record ProductImportProfileDto(
    Guid Id,
    string ProfileName,
    string FileSignature,
    ProductImportMapping Mapping,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ProductImportHistoryDto(
    Guid Id,
    DateTime ImportedAt,
    string UserName,
    string FileName,
    int TotalRecords,
    int InsertedRecords,
    int ExistingRecords,
    int SkippedRecords,
    int ErrorRecords,
    int DurationMilliseconds,
    string Status,
    string? Notes);
