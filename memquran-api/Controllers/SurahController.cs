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
        
        // await Task.Delay(1000); 
        
        _logger.LogInformation("Surah - {Locale}:{SurahNumber} loaded in {Elapsed} ms", locale, surahNumber, sw.Elapsed);
    
        return Ok(new SurahModel { SurahInfo = new SurahInfo { Chapter = int.Parse(surahNumber) } });
    }
    
    public class SurahModel
    {
        public SurahInfo SurahInfo { get; set; }
    }

    public class SurahInfo
    {
        [JsonPropertyName("Chapter")]
        public int Chapter { get; set; }
    }
}