using FiolinOne.Application.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Produces("application/json")]
public sealed class ReportsController(IReportsService reportsService) : ControllerBase
{
    /// <summary>
    /// Ürün, stok, satın alma, üretim ve satış raporlarını tek rapor görünümü için getirir.
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ReportsOverviewDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverview(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        CancellationToken cancellationToken)
    {
        var reports = await reportsService.GetOverviewAsync(
            new ReportsQuery(search, status, dateFrom, dateTo),
            cancellationToken);

        return Ok(reports);
    }
}
