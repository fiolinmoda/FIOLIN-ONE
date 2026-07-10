namespace FiolinOne.Application.MasterData;

public sealed record CreateMasterDataRequest(
    string Name,
    string? Code,
    bool IsActive,
    int? SortOrder);

public sealed record UpdateMasterDataRequest(
    string Name,
    string? Code,
    bool IsActive,
    int? SortOrder);

public sealed record ReorderMasterDataRequest(
    IReadOnlyList<Guid> ItemIds);
