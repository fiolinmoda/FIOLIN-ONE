using FiolinOne.Application.Production;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/production-warehouse-entries")]
[Produces("application/json")]
public sealed class ProductionWarehouseEntriesController(IProductionService productionService) : ControllerBase
{
    /// <summary>Creates a finished goods warehouse entry and completes production.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WarehouseEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEntry([FromBody] CreateWarehouseEntryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var entry = await productionService.CreateWarehouseEntryAsync(request, cancellationToken);
            return Created(string.Empty, entry);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
