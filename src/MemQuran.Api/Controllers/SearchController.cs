using System.Diagnostics;
using MemQuran.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(IStaticFileService staticFileService, ILogger<VersesController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/search/en_search.json
    [HttpGet("/json/search/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/search/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/search/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}