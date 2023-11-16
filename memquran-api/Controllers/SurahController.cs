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
        
        var rootFolder = Path.Combine("..", "..", "memquran-files/json/surahs");
        var fileNameWithoutExtension = $"surah_{surahNumber}";
        var surahsText = await _cache.GetStringAsync(fileNameWithoutExtension);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for GetSurah: {FileName}", fileNameWithoutExtension);
            
            if (!System.IO.File.Exists($"{rootFolder}/{fileNameWithoutExtension}.json"))
            {
                return NotFound();
            }
            
            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{fileNameWithoutExtension}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        _logger.LogInformation("Surah {FileName} text loaded in {Elapsed} ms", fileNameWithoutExtension, sw.Elapsed);
        
        return Ok(surahsText);
    }
    
    [HttpGet("{surahNumber}/translation/{translationId}")]
    public async Task<IActionResult> GetTranslation([FromRoute] int surahNumber, [FromRoute] int translationId)
    {
        var sw = Stopwatch.StartNew();
        
        var rootFolder = Path.Combine("..", "..", "memquran-files/json/surahs");
        var fileNameWithoutExtension = $"translation_{surahNumber}_{translationId}";
        var surahsText = await _cache.GetStringAsync(fileNameWithoutExtension);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for GetTranslation: {FileName}", fileNameWithoutExtension);
            
            if (!System.IO.File.Exists($"{rootFolder}/{fileNameWithoutExtension}.json"))
            {
                return NotFound();
            }
            
            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{fileNameWithoutExtension}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync(fileNameWithoutExtension, surahsText);
        }

        _logger.LogInformation("Translation {FileName} text loaded in {Elapsed} ms", fileNameWithoutExtension, sw.Elapsed);
        
        return Ok(surahsText);
    }
}