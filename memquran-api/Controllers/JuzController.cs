using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class JuzController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<JuzController> _logger;

    public JuzController(IStaticFileService staticFileService, ILogger<JuzController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    // http://localhost:3123/json/juzs/{locale}_juzInfo.json
    // http://localhost:3123/json/juzs/juz_{juzNumber}.json
    // http://localhost:3123/json/juzs/juz_wbw_{locale}_{juzNumber}.json
    [HttpGet("/json/juzs/{fileName}")]
    public async Task<IActionResult> GetJuz([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/juzs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/juzs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://localhost:3123/json/juzTranslations/juz_translation_{juzNumber}_{translationId}.json
    [HttpGet("/json/juzTranslations/{fileName}")]
    public async Task<IActionResult> GetJuzTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/juzTranslations/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/juzTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}