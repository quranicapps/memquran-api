using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    private readonly ILogger<SurahController> _logger;

    public SurahController(ILogger<SurahController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("{languageCode}")]
    public async Task<IActionResult> Get([FromRoute] string languageCode)
    {
        var sw = Stopwatch.StartNew();
        var surahsText = await System.IO.File.ReadAllTextAsync("Resources/Surahs/surahs.json");

        _logger.LogInformation("Surahs text loaded in {Elapsed} ms", sw.Elapsed);
        
        return Ok(surahsText);
    }
}