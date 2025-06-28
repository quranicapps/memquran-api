using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TafsirsController(IStaticFileService staticFileService, ILogger<TafsirsController> logger)
    : Controller
{
    // https://localhost:3123/json/tafsirs/tafsir_{surahNumber}_{tafsirId}.json
    [HttpGet("/json/tafsirs/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/tafsirs/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/tafsirs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}