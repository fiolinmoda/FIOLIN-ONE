using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.MasterData;

public abstract class MasterDataEntity : Entity, IMasterDataEntity
{
    protected MasterDataEntity()
    {
    }

    protected MasterDataEntity(string name, string code, bool isActive, int sortOrder)
    {
        Name = name;
        Code = code;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(string name, string code, bool isActive, int sortOrder)
    {
        Name = name;
        Code = code;
        IsActive = isActive;
        SortOrder = sortOrder;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
