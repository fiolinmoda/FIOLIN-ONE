using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            service = "FIOLIN ONE API",
            status = "Ready",
            timestampUtc = DateTime.UtcNow
        });
    }
}
