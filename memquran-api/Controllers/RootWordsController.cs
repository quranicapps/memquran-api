using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RootWordsController(IStaticFileService staticFileService, ILogger<NamesOfAllahController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/rootWords/en_rootWords.json 
    [HttpGet("/json/rootWords/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await staticFileService.GetFileContentStringAsync($"json/rootWords/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/rootWords/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}