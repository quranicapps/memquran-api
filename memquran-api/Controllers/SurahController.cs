using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<SurahController> _logger;

    public SurahController(IDistributedCache cache, ILogger<SurahController> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    
    [HttpGet("{languageCode}")]
    public async Task<IActionResult> Get([FromRoute] string languageCode)
    {
        var sw = Stopwatch.StartNew();

        var surahsText = await _cache.GetStringAsync($"surahs-{languageCode}");
        if (surahsText is null)
        {
            surahsText = await System.IO.File.ReadAllTextAsync($"Resources/Surahs/surahs-{languageCode}.json");
            await _cache.SetStringAsync($"surahs-{languageCode}", surahsText);
        }

        _logger.LogInformation("Surahs text loaded in {Elapsed} ms", sw.Elapsed);
        
        return Ok(surahsText);
    }
}