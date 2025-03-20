using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PageController(IStaticFileService staticFileService, ILogger<PageController> logger)
    : ControllerBase
{
    // http://localhost:3123/json/pages/{locale}_pageInfo.json
    // http://localhost:3123/json/pages/page_{pageNumber}.json
    // http://localhost:3123/json/pages/page_wbw_{locale}_{pageNumber}.json
    [HttpGet("/json/pages/{fileName}")]
    public async Task<IActionResult> GetPage([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/pages/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/pages/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://localhost:3123/json/pageTranslations/page_translation_{pageNumber}_{translationId}.json
    [HttpGet("/json/pageTranslations/{fileName}")]
    public async Task<IActionResult> GetPageTranslations([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/pageTranslations/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/pageTranslations/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}