using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController(IStaticFileService staticFileService, ILogger<ConfigurationController> logger)
    : ControllerBase
{
    // http://localhost:3123/local/json/settings/recitersOverride.json 
    [HttpGet("/local/json/settings/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var fullFilePath = Path.Combine("..", "..", $"raw/json/settings/{fileName}");
        
        if (!System.IO.File.Exists(fullFilePath))
        {
            return null;
        }
        
        using var streamReader = System.IO.File.OpenText(fullFilePath);
        var text = await streamReader.ReadToEndAsync();

        logger.LogInformation("/local/json/settings/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}