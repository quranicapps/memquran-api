using System.Diagnostics;
using MemQuran.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MaqraController(IStaticFileService staticFileService, ILogger<MaqraController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/maqras/{locale}_maqraInfo.json
    // http://localhost:3123/json/maqras/maqra_{maqraNumber}.json
    // http://localhost:3123/json/maqras/maqra_wbw_{locale}_{maqraNumber}.json
    [HttpGet("/json/maqras/{fileName}")]
    public async Task<IActionResult> GetMaqra([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/maqras/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/maqras/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://localhost:3123/json/maqraTranslations/maqra_translation_{maqraNumber}_{translationId}.json
    [HttpGet("/json/maqraTranslations/{fileName}")]
    public async Task<IActionResult> GetMaqraTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/maqraTranslations/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/maqraTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}