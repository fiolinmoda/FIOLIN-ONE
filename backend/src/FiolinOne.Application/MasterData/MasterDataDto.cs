namespace FiolinOne.Application.MasterData;

public sealed record MasterDataDto(
    Guid Id,
    string Name,
    string Code,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
