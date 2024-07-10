using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(IStaticFileService staticFileService, ILogger<VersesController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/search/en_search.json
    [HttpGet("/json/search/{fileName}")]
    public async Task<IActionResult> GetSearch([FromRoute] string fileName)
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