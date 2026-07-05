using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Fabric;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/fabric-movements")]
[Produces("application/json")]
public sealed class FabricMovementsController(IFabricService fabricService) : ControllerBase
{
    /// <summary>
    /// Gets all fabric stock movements.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FabricMovementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovements(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var movements = await fabricService.GetMovementsAsync(
            new FabricQuery(search, status, sortBy, sortDirection, page, pageSize),
            null,
            cancellationToken);

        return Ok(movements);
    }

    /// <summary>
    /// Creates a manual fabric stock movement.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FabricMovementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMovement([FromBody] CreateFabricMovementRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var movement = await fabricService.CreateMovementAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetMovements), new { id = movement.Id }, movement);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
