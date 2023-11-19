using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class JsonFileController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<JsonFileController> _logger;

    public JsonFileController(IStaticFileService staticFileService, ILogger<JsonFileController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    [HttpGet("/json/{filePath}")]
    public async Task<IActionResult> Get([FromRoute] string filePath)
    {
        var sw = Stopwatch.StartNew();
         
        var text = await _staticFileService.GetFileCommentAsync($"{filePath}");
         
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("SurahInfo text loaded in {Elapsed} ms", sw.Elapsed);
         
        return Ok(text);
    }
}