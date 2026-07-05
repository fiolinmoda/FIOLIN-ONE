namespace FiolinOne.Domain.MasterData;

public interface IMasterDataEntity
{
    Guid Id { get; }
    string Name { get; }
    string Code { get; }
    bool IsActive { get; }
    int SortOrder { get; }
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }

    void Update(string name, string code, bool isActive, int sortOrder);
}
