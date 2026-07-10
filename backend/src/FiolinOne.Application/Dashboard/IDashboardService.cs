namespace FiolinOne.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken);
}
