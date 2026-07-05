namespace FiolinOne.Domain.MasterData;

public sealed class Color : MasterDataEntity
{
    private Color()
    {
    }

    public Color(string name, string code, bool isActive, int sortOrder)
        : base(name, code, isActive, sortOrder)
    {
    }
}
