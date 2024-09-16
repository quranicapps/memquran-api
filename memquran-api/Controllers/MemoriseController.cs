using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MemoriseController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<MemoriseController> _logger;

    public MemoriseController(IStaticFileService staticFileService, ILogger<MemoriseController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3123/json/memorise/en_courses.json 
    // http://localhost:3123/json/memorise/en_questions_2_3.json 
    // http://localhost:3123/json/memorise/en_reviews_2_3.json 
    [HttpGet("/json/memorise/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/memorise/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/memorise/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}