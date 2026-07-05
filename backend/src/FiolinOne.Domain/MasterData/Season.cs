namespace FiolinOne.Domain.MasterData;

public sealed class Season : MasterDataEntity
{
    private Season()
    {
    }

    public Season(string name, string code, bool isActive, int sortOrder)
        : base(name, code, isActive, sortOrder)
    {
    }
}
