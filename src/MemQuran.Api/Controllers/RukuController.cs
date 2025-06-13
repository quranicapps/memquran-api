using System.Diagnostics;
using MemQuran.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RukuController(IStaticFileService staticFileService, ILogger<RukuController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/rukus/{locale}_rukuInfo.json
    // http://localhost:3123/json/rukus/ruku_{rukuNumber}.json
    // http://localhost:3123/json/rukus/ruku_wbw_{locale}_{rukuNumber}.json
    [HttpGet("/json/rukus/{fileName}")]
    public async Task<IActionResult> GetRuku([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/rukus/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/rukus/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://localhost:3123/json/rukuTranslations/ruku_translation_{rukuNumber}_{translationId}.json
    [HttpGet("/json/rukuTranslations/{fileName}")]
    public async Task<IActionResult> GetRukuTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/rukuTranslations/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/rukuTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}