using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VersesController(IStaticFileService staticFileService, ILogger<VersesController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/verses/verses.json
    // http://localhost:3123/json/verses/{locale}_verses.json
    [HttpGet("/json/verses/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/verses/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/verses/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}