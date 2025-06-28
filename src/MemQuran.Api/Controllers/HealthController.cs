using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(IStaticFileService staticFileService, ILogger<HealthController> logger)
    : ControllerBase
{
    // https://localhost:3123/health.json 
    [HttpGet("/health.json")]
    public async Task<IActionResult> Get()
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync("health.json");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("{FileName} loaded in {Elapsed} ms", "health.json", sw.Elapsed);

        return Ok(text);
    }
}