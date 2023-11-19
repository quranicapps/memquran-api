using System.Diagnostics;
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

    [HttpGet("/json/surahs/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var rootFolder = Path.Combine("..", "..", "static/json/surahs");
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var surahsText = await _cache.GetStringAsync(fileNameWithoutExtension);

        if (surahsText is null)
        {
            _logger.LogInformation("***** Cache miss for: {FileNameWithoutExtension}", fileNameWithoutExtension);

            if (!System.IO.File.Exists($"{rootFolder}/{fileNameWithoutExtension}.json"))
            {
                return NotFound();
            }

            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{fileNameWithoutExtension}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await _cache.SetStringAsync(fileNameWithoutExtension, surahsText);
        }

        _logger.LogInformation("{FileName} text loaded in {Elapsed} ms", fileNameWithoutExtension, sw.Elapsed);

        return Ok(surahsText);
    }
}