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
    
    [HttpGet("{surahNumber}")]
    public async Task<IActionResult> Get([FromRoute] int surahNumber)
    {
        var sw = Stopwatch.StartNew();

        var surahsText = await _cache.GetStringAsync($"{surahNumber}");
        
        if (surahsText is null)
        {
            _logger.LogInformation("Cache miss for Surah: {SurahNumber}", surahNumber);
            using var streamReader = System.IO.File.OpenText($"Resources/surahs/{surahNumber}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        _logger.LogInformation("Surah {SurahNumber} text loaded in {Elapsed} ms", surahNumber, sw.Elapsed);
        
        return Ok(surahsText);
    }
}