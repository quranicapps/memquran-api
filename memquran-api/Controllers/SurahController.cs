using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly IHashingService _hashingService;
    private readonly ILogger<SurahController> _logger;

    public SurahController(IDistributedCache cache, IHashingService hashingService, ILogger<SurahController> logger)
    {
        _cache = cache;
        _hashingService = hashingService;
        _logger = logger;
    }
    
    [HttpGet("{surahNumber}")]
    public async Task<IActionResult> GetSurah([FromRoute] int surahNumber)
    {
        var sw = Stopwatch.StartNew();

        var fileName = $"surah_{surahNumber}";
        var surahsText = await _cache.GetStringAsync(fileName);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for GetSurah: {SurahNumber}", surahNumber);
            
            if (!System.IO.File.Exists($"Resources/surahs/{fileName}.json"))
            {
                return NotFound();
            }
            
            using var streamReader = System.IO.File.OpenText($"Resources/surahs/{fileName}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        // _logger.LogInformation("Surah {SurahNumber} text loaded in {Elapsed} ms", surahNumber, sw.Elapsed);
        
        return Ok(surahsText);
    }
    
    [HttpGet("{surahNumber}/translation/{translationId}")]
    public async Task<IActionResult> GetTranslation([FromRoute] int surahNumber, [FromRoute] int translationId)
    {
        var sw = Stopwatch.StartNew();

        var fileName = $"translation_{surahNumber}_{translationId}";
        var surahsText = await _cache.GetStringAsync(fileName);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for GetTranslation: {SurahNumber}", surahNumber);
            
            if (!System.IO.File.Exists($"Resources/surahs/{fileName}.json"))
            {
                return NotFound();
            }
            
            using var streamReader = System.IO.File.OpenText($"Resources/surahs/{fileName}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        // _logger.LogInformation("Surah {SurahNumber} text loaded in {Elapsed} ms", surahNumber, sw.Elapsed);
        
        return Ok(surahsText);
    }
}