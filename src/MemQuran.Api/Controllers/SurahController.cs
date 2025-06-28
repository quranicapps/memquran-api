using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController(IStaticFileService staticFileService, ILogger<SurahController> logger)
    : ControllerBase
{
    // https://localhost:3123/json/surahs/{locale}_surahInfo.json
    // https://localhost:3123/json/surahs/surah_{surahNumber}.json
    // https://localhost:3123/json/surahs/surah_wbw_{locale}_{surahNumber}.json
    [HttpGet("/json/surahs/{fileName}")]
    public async Task<IActionResult> GetSurah([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/surahs/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/surahs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://localhost:3123/json/surahTranslations/surah_translation_{surahNumber}_{translationId}.json
    [HttpGet("/json/surahTranslations/{fileName}")]
    public async Task<IActionResult> GetSurahTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/surahTranslations/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/surahTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}