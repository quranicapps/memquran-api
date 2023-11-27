using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<SurahController> _logger;

    public SurahController(IStaticFileService staticFileService, ILogger<SurahController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    // http://localhost:3000/json/surahs/surah_1.json
    // http://localhost:3000/json/surahs/translation_1_10.json
    [HttpGet("/json/surahs/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileCommentAsync($"json/surahs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}