using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VersesController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<VersesController> _logger;

    public VersesController(IStaticFileService staticFileService, ILogger<VersesController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    // http://localhost:3123/json/verses/verses.json
    // http://localhost:3123/json/verses/{locale}_verses.json
    // http://localhost:3123/json/verses/verses_translation_{translationId}.json
    [HttpGet("/json/verses/{fileName}")]
    public async Task<IActionResult> GetVerses([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/verses/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/verses/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}