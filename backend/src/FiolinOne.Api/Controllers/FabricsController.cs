using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Fabric;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/fabrics")]
[Produces("application/json")]
public sealed class FabricsController(IFabricService fabricService) : ControllerBase
{
    /// <summary>
    /// Gets fabric cards with pagination, filtering, and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FabricDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFabrics(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var fabrics = await fabricService.GetFabricsAsync(
            new FabricQuery(search, status, sortBy, sortDirection, page, pageSize),
            cancellationToken);

        return Ok(fabrics);
    }

    /// <summary>
    /// Gets a fabric card by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FabricDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFabric(Guid id, CancellationToken cancellationToken)
    {
        var fabric = await fabricService.GetFabricAsync(id, cancellationToken);

        return fabric is null ? NotFound() : Ok(fabric);
    }

    /// <summary>
    /// Creates a fabric card.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FabricDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFabric([FromBody] CreateFabricRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fabric = await fabricService.CreateFabricAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetFabric), new { id = fabric.Id }, fabric);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a fabric card.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FabricDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateFabric(Guid id, [FromBody] UpdateFabricRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fabric = await fabricService.UpdateFabricAsync(id, request, cancellationToken);

            return fabric is null ? NotFound() : Ok(fabric);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Soft deletes a fabric card.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFabric(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await fabricService.DeleteFabricAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Records fabric arrival from a fabric supplier and increases stock by total weight.
    /// </summary>
    [HttpPost("purchase-arrivals")]
    [ProducesResponseType(typeof(FabricMovementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePurchaseArrival(
        [FromBody] CreateFabricPurchaseMovementRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var movement = await fabricService.CreatePurchaseMovementAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetMovements), new { fabricId = movement.FabricId }, movement);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Consumes fabric for production and decreases stock.
    /// </summary>
    [HttpPost("consumption")]
    [ProducesResponseType(typeof(FabricMovementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConsumeFabric([FromBody] CreateFabricConsumptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var movement = await fabricService.ConsumeFabricAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetMovements), new { fabricId = movement.FabricId }, movement);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Gets movement history for one fabric card.
    /// </summary>
    [HttpGet("{fabricId:guid}/movements")]
    [ProducesResponseType(typeof(PagedResult<FabricMovementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovements(
        Guid fabricId,
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
            fabricId,
            cancellationToken);

        return Ok(movements);
    }
}
