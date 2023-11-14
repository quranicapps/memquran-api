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
    public async Task<IActionResult> Get([FromRoute] int surahNumber)
    {
        var sw = Stopwatch.StartNew();

        var translations = new List<int>(); // Get from queryString
        var sb = new StringBuilder($"surah_{surahNumber}");
        if (translations.Any())
        {
            sb.Append($"&tr={string.Join(",", translations.OrderBy(x => x))}");
        }

        var fileNameWithoutExtension = _hashingService.ToHashString(sb.ToString());
        
        var surahsText = await _cache.GetStringAsync(fileNameWithoutExtension);
        
        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for Surah: {SurahNumber}", surahNumber);
            using var streamReader = System.IO.File.OpenText($"Resources/surahs/{fileNameWithoutExtension}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync($"{surahNumber}", surahsText);
        }

        // _logger.LogInformation("Surah {SurahNumber} text loaded in {Elapsed} ms", surahNumber, sw.Elapsed);
        
        return Ok(surahsText);
    }
}