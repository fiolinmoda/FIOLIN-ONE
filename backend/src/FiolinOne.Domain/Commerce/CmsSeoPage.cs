using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Commerce;

public sealed class CmsSeoPage : Entity
{
    private CmsSeoPage()
    {
    }

    public CmsSeoPage(
        string route,
        string metaTitle,
        string metaDescription,
        string? canonical,
        string? openGraphTitle,
        string? openGraphDescription,
        string? twitterCard,
        string? schemaJson)
    {
        Route = route;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        Canonical = canonical;
        OpenGraphTitle = openGraphTitle;
        OpenGraphDescription = openGraphDescription;
        TwitterCard = twitterCard;
        SchemaJson = schemaJson;
    }

    public string Route { get; private set; } = string.Empty;
    public string MetaTitle { get; private set; } = string.Empty;
    public string MetaDescription { get; private set; } = string.Empty;
    public string? Canonical { get; private set; }
    public string? OpenGraphTitle { get; private set; }
    public string? OpenGraphDescription { get; private set; }
    public string? TwitterCard { get; private set; }
    public string? SchemaJson { get; private set; }
}
