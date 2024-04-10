using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RukuController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<RukuController> _logger;

    public RukuController(IStaticFileService staticFileService, ILogger<RukuController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3000/json/rukuInfos/{locale}_rukuInfo.json
    [HttpGet("/json/rukuInfos/{fileName}")]
    public async Task<IActionResult> GetRukuInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/rukuInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/rukuInfos/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }

    // http://localhost:3000/json/rukus/ruku_{rukuNumber}.json
    // http://localhost:3000/json/rukus/ruku_translation_{rukuNumber}_{translationId}.json
    [HttpGet("/json/rukus/{fileName}")]
    public async Task<IActionResult> GetRuku([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/rukus/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/rukus/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}