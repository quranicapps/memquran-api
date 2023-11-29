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
    
    // http://localhost:3000/json/juzInfos/{locale}_juzInfo.json
    [HttpGet("/json/juzInfos/{fileName}")]
    public async Task<IActionResult> GetSurahInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/juzInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("{fileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }

    // http://localhost:3000/json/juzs/juz_{chapter}.json
    // http://localhost:3000/json/juzs/translation_{juz}_{translationId}.json
    [HttpGet("/json/juzs/{fileName}")]
    public async Task<IActionResult> GetJuz([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/juzs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}