using FiolinOne.Application.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>
    /// İş sahibinin sabah kontrolü için ana panel özetini getirir.
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(DashboardOverviewDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        return Ok(await dashboardService.GetOverviewAsync(cancellationToken));
    }
}
