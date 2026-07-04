namespace FiolinOne.Domain.MasterData;

public sealed class Size : MasterDataEntity
{
    private Size()
    {
    }

    public Size(string name, string code, bool isActive, int sortOrder)
        : base(name, code, isActive, sortOrder)
    {
    }
}
