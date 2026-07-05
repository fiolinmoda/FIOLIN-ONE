using FiolinOne.Application.Production;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/production-cutting")]
[Produces("application/json")]
public sealed class ProductionCuttingController(IProductionService productionService) : ControllerBase
{
    /// <summary>Creates a cutting record and consumes fabric stock.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CuttingRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCutting([FromBody] CreateCuttingRecordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var record = await productionService.CreateCuttingAsync(request, cancellationToken);
            return Created(string.Empty, record);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
