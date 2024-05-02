using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DuasController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<DuasController> _logger;

    public DuasController(IStaticFileService staticFileService, ILogger<DuasController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3123/json/duas/en_duas.json 
    [HttpGet("/json/duas/{fileName}")]
    public async Task<IActionResult> GetDuas([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/duas/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/duas/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}