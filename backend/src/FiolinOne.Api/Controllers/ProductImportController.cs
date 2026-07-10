using System.Security.Claims;
using System.Text.Json;
using FiolinOne.Application.Products.Import;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiolinOne.Api.Controllers;

[ApiController]
[Route("api/products/import")]
[Produces("application/json")]
[Authorize(Roles = "Administrator")]
public sealed class ProductImportController(IProductImportService productImportService) : ControllerBase
{
    /// <summary>
    /// Excel dosyasını okur ve kayıt oluşturmadan önizleme üretir.
    /// </summary>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(ProductImportPreviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Preview([FromForm] IFormFile file, [FromForm] string? options, CancellationToken cancellationToken)
    {
        try
        {
            var request = Deserialize<ProductImportPreviewRequest>(options)
                ?? new ProductImportPreviewRequest(null, "Cancel");

            await using var stream = file.OpenReadStream();
            return Ok(await productImportService.PreviewAsync(stream, file.FileName, request, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (JsonException)
        {
            return BadRequest(new { message = "İçe aktarma seçenekleri okunamadı." });
        }
    }

    /// <summary>
    /// Onaylanan Excel içe aktarma işlemini çalıştırır.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Import([FromForm] IFormFile file, [FromForm] string options, CancellationToken cancellationToken)
    {
        try
        {
            var request = Deserialize<ProductImportExecuteRequest>(options)
                ?? throw new InvalidOperationException("İçe aktarma seçenekleri okunamadı.");

            await using var stream = file.OpenReadStream();
            return Ok(await productImportService.ImportAsync(stream, file.FileName, UserName(), request, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (JsonException)
        {
            return BadRequest(new { message = "İçe aktarma seçenekleri okunamadı." });
        }
    }

    /// <summary>
    /// Ürün Excel import geçmişini listeler.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductImportHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        return Ok(await productImportService.GetHistoryAsync(cancellationToken));
    }

    private string UserName()
    {
        return User.Identity?.Name
            ?? User.FindFirstValue(ClaimTypes.Name)
            ?? "Administrator";
    }

    private static T? Deserialize<T>(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? default
            : JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}
