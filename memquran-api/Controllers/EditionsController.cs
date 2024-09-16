using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EditionsController(IStaticFileService staticFileService, ILogger<VersesController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/editions/editions.json
    [HttpGet("/json/editions/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/editions/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/editions/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}