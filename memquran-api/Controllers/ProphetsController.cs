using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProphetsController(IStaticFileService staticFileService, ILogger<ProphetsController> logger)
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