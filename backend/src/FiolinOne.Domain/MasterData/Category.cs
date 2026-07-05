namespace FiolinOne.Domain.MasterData;

public sealed class Category : MasterDataEntity
{
    private Category()
    {
    }

    public Category(string name, string code, bool isActive, int sortOrder)
        : base(name, code, isActive, sortOrder)
    {
    }
}
