using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class NamesOfAllahController(IStaticFileService staticFileService, ILogger<NamesOfAllahController> logger)
    : ControllerBase
{
    // https://localhost:3123/json/namesOfAllah/en_names.json 
    [HttpGet("/json/namesOfAllah/{fileName}")]
    public async Task<IActionResult> Get([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/namesOfAllah/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/namesOfAllah/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);

        return Ok(text);
    }
}