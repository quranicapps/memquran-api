using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NamesOfAllahController(IStaticFileService staticFileService, ILogger<NamesOfAllahController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/namesOfAllah/en_names.json 
    [HttpGet("/json/namesOfAllah/{fileName}")]
    public async Task<IActionResult> GetNamesOfAllah([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await staticFileService.GetFileContentStringAsync($"json/namesOfAllah/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/namesOfAllah/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}