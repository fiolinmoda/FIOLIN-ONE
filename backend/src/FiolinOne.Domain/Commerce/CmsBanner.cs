using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Commerce;

public sealed class CmsBanner : Entity
{
    private CmsBanner()
    {
    }

    public CmsBanner(string title, string? subtitle, string imageUrl, string? linkUrl, string placement, bool isActive, int sortOrder)
    {
        Title = title;
        Subtitle = subtitle;
        ImageUrl = imageUrl;
        LinkUrl = linkUrl;
        Placement = placement;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public string Title { get; private set; } = string.Empty;
    public string? Subtitle { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string? LinkUrl { get; private set; }
    public string Placement { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
}
