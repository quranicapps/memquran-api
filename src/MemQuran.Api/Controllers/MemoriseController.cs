using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MemoriseController(IStaticFileService staticFileService, ILogger<MemoriseController> logger)
    : ControllerBase
{
    // https://localhost:3123/json/memorise/en_courses.json 
    // https://localhost:3123/json/memorise/en_questions_2_3.json 
    // https://localhost:3123/json/memorise/en_reviews_2_3.json 
    [HttpGet("/json/memorise/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/memorise/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/memorise/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}