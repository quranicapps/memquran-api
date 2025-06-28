using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JuzController(IStaticFileService staticFileService, ILogger<JuzController> logger)
    : ControllerBase
{
    // https://localhost:3123/json/juzs/{locale}_juzInfo.json
    // https://localhost:3123/json/juzs/juz_{juzNumber}.json
    // https://localhost:3123/json/juzs/juz_wbw_{locale}_{juzNumber}.json
    [HttpGet("/json/juzs/{fileName}")]
    public async Task<IActionResult> GetJuz([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/juzs/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/juzs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://localhost:3123/json/juzTranslations/juz_translation_{juzNumber}_{translationId}.json
    [HttpGet("/json/juzTranslations/{fileName}")]
    public async Task<IActionResult> GetJuzTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/juzTranslations/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/juzTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}