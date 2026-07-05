using FiolinOne.Application.MasterData;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/master-data/{type}")]
[Produces("application/json")]
public sealed class MasterDataController(IMasterDataService masterDataService) : ControllerBase
{
    /// <summary>
    /// Gets master data items by type.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MasterDataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetItems(string type, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        try
        {
            var items = await masterDataService.GetItemsAsync(type, search, cancellationToken);

            return Ok(items);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Gets a master data item by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MasterDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItem(string type, Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await masterDataService.GetItemAsync(type, id, cancellationToken);

            return item is null ? NotFound() : Ok(item);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Creates a master data item.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MasterDataDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateItem(
        string type,
        [FromBody] CreateMasterDataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await masterDataService.CreateItemAsync(type, request, cancellationToken);

            return CreatedAtAction(nameof(GetItem), new { type, id = item.Id }, item);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Updates a master data item.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MasterDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateItem(
        string type,
        Guid id,
        [FromBody] UpdateMasterDataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await masterDataService.UpdateItemAsync(type, id, request, cancellationToken);

            return item is null ? NotFound() : Ok(item);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    /// <summary>
    /// Deletes a master data item.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(string type, Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await masterDataService.DeleteItemAsync(type, id, cancellationToken);

            return deleted ? NoContent() : NotFound();
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
