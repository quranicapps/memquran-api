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
    public async Task<IActionResult> GetJuzInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/juzInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/juzInfos/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }

    // http://localhost:3000/json/juzs/juz_{juzNumber}.json
    // http://localhost:3000/json/juzs/juz_translation_{juzNumber}_{translationId}.json
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
}