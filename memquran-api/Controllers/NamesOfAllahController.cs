using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NamesOfAllahController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<NamesOfAllahController> _logger;

    public NamesOfAllahController(IStaticFileService staticFileService, ILogger<NamesOfAllahController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3123/json/99NamesOfAllah/en_names.json 
    [HttpGet("/json/99NamesOfAllah/{fileName}")]
    public async Task<IActionResult> GetNamesOfAllah([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/99NamesOfAllah/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/99NamesOfAllah/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}