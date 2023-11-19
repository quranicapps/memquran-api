using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController(IDistributedCache cache, ILogger<SurahController> logger)
    : ControllerBase
{
    [HttpGet("/json/surahs/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var rootFolder = Path.Combine("..", "..", "static/json/surahs");
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var surahsText = await cache.GetStringAsync(fileNameWithoutExtension);

        if (surahsText is null)
        {
            logger.LogInformation("***** Cache miss for: {FileNameWithoutExtension}", fileNameWithoutExtension);

            if (!System.IO.File.Exists($"{rootFolder}/{fileNameWithoutExtension}.json"))
            {
                return NotFound();
            }

            using var streamReader = System.IO.File.OpenText($"{rootFolder}/{fileNameWithoutExtension}.json");
            surahsText = await streamReader.ReadToEndAsync();
            await cache.SetStringAsync(fileNameWithoutExtension, surahsText);
        }

        logger.LogInformation("{FileName} text loaded in {Elapsed} ms", fileNameWithoutExtension, sw.Elapsed);

        return Ok(surahsText);
    }
}