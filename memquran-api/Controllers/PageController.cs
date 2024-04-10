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
    
    // http://localhost:3000/json/pageInfos/{locale}_pageInfo.json
    [HttpGet("/json/pageInfos/{fileName}")]
    public async Task<IActionResult> GetPageInfos([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/pageInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/pageInfos/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }

    // http://localhost:3000/json/pages/page_{pageNumber}.json
    // http://localhost:3000/json/pages/page_translation_{pageNumber}_{translationId}.json
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
}