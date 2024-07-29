using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TafsirsController : Controller
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<TafsirsController> _logger;

    public TafsirsController(IStaticFileService staticFileService, ILogger<TafsirsController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3123/json/tafsirs/tafsir{surahNumber}_{tafsirId}.json
    [HttpGet("/json/tafsirs/{fileName}")]
    public async Task<IActionResult> GetSurah([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/tafsirs/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/tafsirs/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}