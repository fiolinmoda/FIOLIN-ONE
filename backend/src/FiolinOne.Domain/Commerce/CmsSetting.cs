using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Commerce;

public sealed class CmsSetting : Entity
{
    private CmsSetting()
    {
    }

    public CmsSetting(string key, string value, string group)
    {
        Key = key;
        Value = value;
        Group = group;
    }

    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string Group { get; private set; } = string.Empty;
}
