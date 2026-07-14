namespace FiolinOne.Application.Commerce;

public sealed record GetCommerceHomeQuery;
public sealed record GetCommerceCategoriesQuery;
public sealed record GetCommerceProductsQuery(string? Search);
public sealed record GetCommerceProductQuery(string Slug);
public sealed record GetCommerceMenuQuery;
public sealed record GetCommerceBannerQuery;
public sealed record GetCommerceSliderQuery;
public sealed record GetCommerceSettingsQuery;
