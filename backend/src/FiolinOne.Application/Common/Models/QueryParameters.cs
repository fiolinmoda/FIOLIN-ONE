namespace FiolinOne.Application.Common.Models;

public sealed record QueryParameters(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 25);
