using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Fabric;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/fabric-reservations")]
[Produces("application/json")]
public sealed class FabricReservationsController(IFabricService fabricService) : ControllerBase
{
    /// <summary>
    /// Gets fabric reservations with pagination, filtering, and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FabricReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservations(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var reservations = await fabricService.GetReservationsAsync(
            new FabricQuery(search, status, sortBy, sortDirection, page, pageSize),
            cancellationToken);

        return Ok(reservations);
    }

    /// <summary>
    /// Creates a fabric reservation for production.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FabricReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation(
        [FromBody] CreateFabricReservationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reservation = await fabricService.CreateReservationAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetReservations), new { id = reservation.Id }, reservation);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a fabric reservation.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FabricReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateReservation(
        Guid id,
        [FromBody] UpdateFabricReservationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reservation = await fabricService.UpdateReservationAsync(id, request, cancellationToken);

            return reservation is null ? NotFound() : Ok(reservation);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Soft deletes a fabric reservation.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReservation(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await fabricService.DeleteReservationAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
