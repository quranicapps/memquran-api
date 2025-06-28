using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EditionsController(IStaticFileService staticFileService, ILogger<VersesController> logger)
    : ControllerBase
{
    // https://localhost:3123/json/editions/tafsirEditions.json
    // https://localhost:3123/json/editions/translationEditions.json
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