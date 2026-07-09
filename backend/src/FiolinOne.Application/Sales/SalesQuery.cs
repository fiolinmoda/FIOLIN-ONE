using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Sales;

public sealed record SalesQuery(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 25)
{
    public QueryParameters ToParameters()
    {
        return new QueryParameters(Search, Status, SortBy, SortDirection, Page, PageSize);
    }
}
