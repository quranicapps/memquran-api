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
        var rootFolder = Path.Combine("..", "..", "Data/QuranData/surahs");
        var sw = Stopwatch.StartNew();

        var fileName = $"surah_{surahNumber}";
        var surahsText = await _cache.GetStringAsync(fileName);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for GetSurah: {FileName}", fileName);
            
            if (!System.IO.File.Exists($"{rootFolder}/{fileName}.json"))
            {
                return NotFound();
            }
            
            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{fileName}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        _logger.LogInformation("Surah {FileName} text loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(surahsText);
    }
    
    [HttpGet("{surahNumber}/translation/{translationId}")]
    public async Task<IActionResult> GetTranslation([FromRoute] int surahNumber, [FromRoute] int translationId)
    {
        var rootFolder = Path.Combine("..", "..", "Data/QuranData/surahs");
        var sw = Stopwatch.StartNew();

        var fileName = $"translation_{surahNumber}_{translationId}";
        var surahsText = await _cache.GetStringAsync(fileName);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for GetTranslation: {FileName}", fileName);
            
            if (!System.IO.File.Exists($"{rootFolder}/{fileName}.json"))
            {
                return NotFound();
            }
            
            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{fileName}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        _logger.LogInformation("Translation {FileName} text loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(surahsText);
    }
}