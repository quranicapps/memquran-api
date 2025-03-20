using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ReciterController(IStaticFileService staticFileService, ILogger<ReciterController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/reciterInfos/ab_abdallah-al-matroud-1.json
    [HttpGet("/json/reciterInfos/{fileName}")]
    public async Task<IActionResult> GetReciterInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/reciterInfos/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/reciterInfos/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://localhost:3123/json/reciters/en_reciters.json
    [HttpGet("/json/reciters/{fileName}")]
    public async Task<IActionResult> GetReciters([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/reciters/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/reciters/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}