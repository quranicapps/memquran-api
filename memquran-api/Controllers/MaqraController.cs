using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MaqraController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<MaqraController> _logger;

    public MaqraController(IStaticFileService staticFileService, ILogger<MaqraController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    // http://localhost:3123/json/maqras/{locale}_maqraInfo.json
    // http://localhost:3123/json/maqras/maqra_{maqraNumber}.json
    // http://localhost:3123/json/maqras/maqra_wbw_{locale}_{maqraNumber}.json
    [HttpGet("/json/maqras/{fileName}")]
    public async Task<IActionResult> GetMaqra([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/maqras/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/maqras/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://localhost:3123/json/maqraTranslations/maqra_translation_{maqraNumber}_{translationId}.json
    [HttpGet("/json/maqraTranslations/{fileName}")]
    public async Task<IActionResult> GetMaqraTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/maqraTranslations/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/maqraTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}