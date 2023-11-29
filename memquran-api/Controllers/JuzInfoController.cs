using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class JuzInfoController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<JuzInfoController> _logger;

    public JuzInfoController(IStaticFileService staticFileService, ILogger<JuzInfoController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }

    // http://localhost:3000/json/juzInfos/en_juzInfo.json
    [HttpGet("/json/juzInfos/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentAsync($"json/juzInfos/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("{fileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return Ok(text);
    }
}