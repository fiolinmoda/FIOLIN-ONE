using FiolinOne.Application.Production;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/workshop-shipments")]
[Produces("application/json")]
public sealed class WorkshopShipmentsController(IProductionService productionService) : ControllerBase
{
    /// <summary>Creates a workshop shipment record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkshopShipmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateShipment([FromBody] CreateWorkshopShipmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var shipment = await productionService.CreateWorkshopShipmentAsync(request, cancellationToken);
            return Created(string.Empty, shipment);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
