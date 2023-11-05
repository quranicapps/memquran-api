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
    
    [HttpGet("{locale}")]
    public async Task<IActionResult> Get([FromRoute] string locale)
    {
        var sw = Stopwatch.StartNew();

        var surahsText = await _cache.GetStringAsync($"{locale}_surahInfos");
        
        if (surahsText is null)
        {
            _logger.LogInformation("Cache miss for {Locale}_surahInfos", locale);
            using var streamReader = System.IO.File.OpenText($"Resources/surahInfos/{locale}_surahInfos.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{locale}_surahInfos", surahsText);
        }

        _logger.LogInformation("SurahInfos text loaded in {Elapsed} ms", sw.Elapsed);
        
        return Ok(surahsText);
    }
}