using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahInfoController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<SurahInfoController> _logger;

    public SurahInfoController(IDistributedCache cache, ILogger<SurahInfoController> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    
    [HttpGet("{languageCode}")]
    public async Task<IActionResult> Get([FromRoute] string languageCode)
    {
        var sw = Stopwatch.StartNew();

        var surahsText = await _cache.GetAsync($"surahs-{languageCode}");
        if (surahsText is null)
        {
            surahsText = await System.IO.File.ReadAllBytesAsync($"Resources/surahInfo/{languageCode}_surahInfos.json");
            await _cache.SetAsync($"surahInfos-{languageCode}", surahsText);
        }

        _logger.LogInformation("SurahInfos text loaded in {Elapsed} ms", sw.Elapsed);
        
        return Ok(Encoding.UTF8.GetString(surahsText));
    }
}