using FiolinOne.Application.Production;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/production-orders/{productionOrderId:guid}/timeline")]
[Produces("application/json")]
public sealed class ProductionTimelineController(IProductionService productionService) : ControllerBase
{
    /// <summary>Gets production timeline entries.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductionTimelineDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeline(Guid productionOrderId, CancellationToken cancellationToken)
    {
        return Ok(await productionService.GetTimelineAsync(productionOrderId, cancellationToken));
    }
}
