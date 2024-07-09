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
    
    // http://localhost:3123/json/surahs/{locale}_surahInfo.json
    // http://localhost:3123/json/surahs/surah_{surahNumber}.json
    // http://localhost:3123/json/surahs/surah_wbw_{locale}_{surahNumber}.json
    [HttpGet("/json/surahs/{fileName}")]
    public async Task<IActionResult> GetSurah([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/surahs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/surahs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://localhost:3123/json/surahTranslations/surah_wbw_{locale}_{surahNumber}.json
    [HttpGet("/json/surahTranslations/{fileName}")]
    public async Task<IActionResult> GetSurahTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/surahTranslations/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/surahTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}