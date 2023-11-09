using System.Diagnostics;
using System.Text.Json.Serialization;
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
    
    [HttpGet("{locale}/{surahNumber}")]
    public async Task<IActionResult> Get([FromRoute] string locale, string surahNumber)
    {
        var sw = Stopwatch.StartNew();

        var surahsText = await _cache.GetStringAsync($"{locale}_surah_{surahNumber}");
        
        if (surahsText is null)
        {
            _logger.LogInformation("Cache miss for {Locale}_surah_{SurahNumber}", locale, surahNumber);
            using var streamReader = System.IO.File.OpenText($"Resources/surahs/{locale}/{locale}_surah_{surahNumber}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{locale}_surah_{surahNumber}", surahsText);
        }

        _logger.LogInformation("Surah {Locale}_surah_{SurahNumber} text loaded in {Elapsed} ms", locale, surahNumber, sw.Elapsed);
        
        return Ok(surahsText);
    }
}