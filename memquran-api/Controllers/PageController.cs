using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PageController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<PageController> _logger;

    public PageController(IStaticFileService staticFileService, ILogger<PageController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    // http://localhost:3123/json/pages/{locale}_pageInfo.json
    // http://localhost:3123/json/pages/page_{pageNumber}.json
    // http://localhost:3123/json/pages/page_wbw_{locale}_{pageNumber}.json
    [HttpGet("/json/pages/{fileName}")]
    public async Task<IActionResult> GetPage([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/pages/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/pages/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://localhost:3123/json/pageTranslations/page_translation_{pageNumber}_{translationId}.json
    [HttpGet("/json/pageTranslations/{fileName}")]
    public async Task<IActionResult> GetPageTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/pageTranslations/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/pageTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}