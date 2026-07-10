namespace FiolinOne.Application.Reports;

public interface IReportsService
{
    Task<ReportsOverviewDto> GetOverviewAsync(ReportsQuery query, CancellationToken cancellationToken);
}
