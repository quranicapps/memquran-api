using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RecitersVbvUseCustomDictionaryController(IStaticFileService staticFileService, ILogger<RecitersVbvUseCustomDictionaryController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/recitersVbvUseCustom/recitersVbvUseCustomDictionary.json
    [HttpGet("/json/recitersVbvUseCustom/{fileName}")]
    public async Task<IActionResult> GetRecitersVbvUseCustom([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/recitersVbvUseCustom/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/recitersVbvUseCustom/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}