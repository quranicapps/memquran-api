using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<SurahController> _logger;

    public SurahController(IStaticFileService staticFileService, ILogger<SurahController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3000/json/surahInfos/{locale}_surahInfo.json
    [HttpGet("/json/surahInfos/{fileName}")]
    public async Task<IActionResult> GetSurahInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/surahInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/surahInfos/{fileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }

    // http://localhost:3000/json/surahs/surah_{chapter}.json
    // http://localhost:3000/json/surahs/surah_translation_{chapter}_{translationId}.json
    [HttpGet("/json/surahs/{fileName}")]
    public async Task<IActionResult> GetSurah([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/surahs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/surahs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}