using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Commerce;

public sealed class CmsMenu : Entity
{
    private CmsMenu()
    {
    }

    public CmsMenu(string title, string url, string location, bool isActive, int sortOrder)
    {
        Title = title;
        Url = url;
        Location = location;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public string Title { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
}
