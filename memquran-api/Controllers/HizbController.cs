using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HizbController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<HizbController> _logger;

    public HizbController(IStaticFileService staticFileService, ILogger<HizbController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3000/json/hizbInfos/{locale}_hizbInfo.json
    [HttpGet("/json/hizbInfos/{fileName}")]
    public async Task<IActionResult> GetHizbInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/hizbInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/hizbInfos/{fileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }

    // http://localhost:3000/json/hizbs/hizb_{hizbNumber}.json
    // http://localhost:3000/json/hizbs/hizb_translation_{hizbNumber}_{translationId}.json
    [HttpGet("/json/hizbs/{fileName}")]
    public async Task<IActionResult> GetHizb([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/hizbs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/hizbs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}