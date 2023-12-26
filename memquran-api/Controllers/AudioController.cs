using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AudioController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<AudioController> _logger;

    public AudioController(IStaticFileService staticFileService, ILogger<AudioController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3000/json/audio/abderahmane-eloosi-1/timings/surah/abderahmane-eloosi-1_timings_surah_1.json
    [HttpGet("/json/audio/{reciterId}/timings/surah/{fileName}")]
    public async Task<IActionResult> GetSurahAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/surah/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/surah/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://localhost:3000/json/audio/abu-bakr-ash-shaatree-1/timings/juz/abu-bakr-ash-shaatree-1_timings_juz_1.json
    [HttpGet("/json/audio/{reciterId}/timings/juz/{fileName}")]
    public async Task<IActionResult> GetJuzAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/juz/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/juz/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);
        
        return Ok(text);
    }
}