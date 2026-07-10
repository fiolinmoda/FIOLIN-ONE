using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Products;

public sealed class ProductImportHistory : Entity
{
    private ProductImportHistory()
    {
    }

    public ProductImportHistory(
        string userName,
        string fileName,
        int totalRecords,
        int insertedRecords,
        int existingRecords,
        int skippedRecords,
        int errorRecords,
        int durationMilliseconds,
        string status,
        string? notes)
    {
        UserName = userName;
        FileName = fileName;
        TotalRecords = totalRecords;
        InsertedRecords = insertedRecords;
        ExistingRecords = existingRecords;
        SkippedRecords = skippedRecords;
        ErrorRecords = errorRecords;
        DurationMilliseconds = durationMilliseconds;
        Status = status;
        Notes = notes;
    }

    public string UserName { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public int TotalRecords { get; private set; }
    public int InsertedRecords { get; private set; }
    public int ExistingRecords { get; private set; }
    public int SkippedRecords { get; private set; }
    public int ErrorRecords { get; private set; }
    public int DurationMilliseconds { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public DateTime ImportedAt => CreatedAtUtc;
}
