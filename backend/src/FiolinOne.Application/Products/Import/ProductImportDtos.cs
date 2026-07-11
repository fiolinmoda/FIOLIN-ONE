namespace FiolinOne.Application.Products.Import;

public sealed record ProductImportMapping(
    string? ModelCode,
    string? Barcode,
    string? ProductName,
    string? Brand,
    string? Category,
    string? Season,
    string? Color,
    string? Size,
    string? FabricType,
    string? PurchasePrice,
    string? SalesPrice,
    string? Stock,
    string? ImageUrl);

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
    ProductImportMissingMasterDataDto MissingMasterData,
    IReadOnlyList<ProductImportPreviewRowDto> Rows);

public sealed record ProductImportSummaryDto(
    int Total,
    int Valid,
    int MissingField,
    int Error,
    int NewProducts,
    int ExistingProducts,
    int Skipped);

public sealed record ProductImportMissingMasterDataDto(
    IReadOnlyList<string> Brands,
    IReadOnlyList<string> Categories,
    IReadOnlyList<string> Seasons,
    IReadOnlyList<string> Colors,
    IReadOnlyList<string> Sizes,
    IReadOnlyList<string> FabricTypes);

public sealed record ProductImportCreatedMasterDataDto(
    int Brands,
    int Categories,
    int Seasons,
    int Colors,
    int Sizes,
    int FabricTypes);

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
    ProductImportCreatedMasterDataDto CreatedMasterData,
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
