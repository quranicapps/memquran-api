using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahInfoController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<SurahInfoController> _logger;

    public SurahInfoController(IStaticFileService staticFileService, ILogger<SurahInfoController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    [HttpGet("/json/surahInfos/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileCommentAsync($"json/surahInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("{fileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}