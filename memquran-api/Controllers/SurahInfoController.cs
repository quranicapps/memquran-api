using System.Diagnostics;
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
        
        var rootFolder = Path.Combine("..", "..", "memquran-files/json/surahInfos");
        var fileNameWithoutExtension = $"{locale}_surahInfo";
        var surahsText = await _cache.GetStringAsync(fileNameWithoutExtension);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for {Locale}_surahInfo", locale);
            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{locale}_surahInfo.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync(fileNameWithoutExtension, surahsText);
        }

        // _logger.LogInformation("SurahInfo text loaded in {Elapsed} ms", sw.Elapsed);
        
        return Ok(surahsText);
    }
}