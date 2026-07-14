using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Commerce;

public sealed class CmsSlider : Entity
{
    private CmsSlider()
    {
    }

    public CmsSlider(string title, string? subtitle, string imageUrl, string? linkUrl, bool isActive, int sortOrder)
    {
        Title = title;
        Subtitle = subtitle;
        ImageUrl = imageUrl;
        LinkUrl = linkUrl;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public string Title { get; private set; } = string.Empty;
    public string? Subtitle { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string? LinkUrl { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
}
