using System.Diagnostics;
using MemQuran.Api.Settings;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProphetsController(
    IStaticFileService staticFileService,
    ILogger<ProphetsController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/prophets/en_prophets.json 
    [HttpGet("/json/prophets/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/prophets/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/prophets/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}