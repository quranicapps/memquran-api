using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DuasController(IStaticFileService staticFileService, ILogger<DuasController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/duas/en_duas.json 
    [HttpGet("/json/duas/{fileName}")]
    public async Task<IActionResult> GetDuas([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/duas/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/duas/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}