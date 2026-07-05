using FiolinOne.Application.Production;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/workshop-returns")]
[Produces("application/json")]
public sealed class WorkshopReturnsController(IProductionService productionService) : ControllerBase
{
    /// <summary>Creates a workshop return record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkshopReturnDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReturn([FromBody] CreateWorkshopReturnRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var workshopReturn = await productionService.CreateWorkshopReturnAsync(request, cancellationToken);
            return Created(string.Empty, workshopReturn);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
