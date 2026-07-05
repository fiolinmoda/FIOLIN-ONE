namespace FiolinOne.Domain.MasterData;

public sealed class Brand : MasterDataEntity
{
    private Brand()
    {
    }

    public Brand(string name, string code, bool isActive, int sortOrder)
        : base(name, code, isActive, sortOrder)
    {
    }
}
