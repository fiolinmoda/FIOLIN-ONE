namespace FiolinOne.Domain.MasterData;

public sealed class FabricType : MasterDataEntity
{
    private FabricType()
    {
    }

    public FabricType(string name, string code, bool isActive, int sortOrder)
        : base(name, code, isActive, sortOrder)
    {
    }
}
