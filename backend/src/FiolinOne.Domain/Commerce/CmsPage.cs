using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Commerce;

public sealed class CmsPage : Entity
{
    private CmsPage()
    {
    }

    public CmsPage(string title, string slug, string content, bool isPublished, int sortOrder)
    {
        Title = title;
        Slug = slug;
        Content = content;
        IsPublished = isPublished;
        SortOrder = sortOrder;
    }

    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsPublished { get; private set; }
    public int SortOrder { get; private set; }
}
