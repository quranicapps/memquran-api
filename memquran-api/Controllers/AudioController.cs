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
    
    // http://localhost:3123/json/audio/abderahmane-eloosi-1/timings/surah/abderahmane-eloosi-1_timings_surah_1.json
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
    
    // http://localhost:3123/json/audio/abu-bakr-ash-shaatree-1/timings/juz/abu-bakr-ash-shaatree-1_timings_juz_1.json
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
    
    // http://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/page/khalifah-al-tunaiji-161-3_timings_page_604.json
    [HttpGet("/json/audio/{reciterId}/timings/page/{fileName}")]
    public async Task<IActionResult> GetPageAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/page/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/page/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/ruku/khalifah-al-tunaiji-161-3_timings_ruku_300.json
    [HttpGet("/json/audio/{reciterId}/timings/ruku/{fileName}")]
    public async Task<IActionResult> GetRukuAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/ruku/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/ruku/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/maqra/khalifah-al-tunaiji-161-3_timings_maqra_300.json
    [HttpGet("/json/audio/{reciterId}/timings/maqra/{fileName}")]
    public async Task<IActionResult> GetMaqraAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();
        
        var text = await _staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/maqra/{fileName}");
        
        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/maqra/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);
        
        return Ok(text);
    }
    
    // http://localhost:3123/audio/tajweed/samples/ikhfa.mp3
    [HttpGet("/audio/tajweed/samples/{fileName}")]
    public async Task<IActionResult> GetTajweedAudio([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"audio/tajweed/samples/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/audio/tajweed/samples/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "audio/mp3");
    }
    
    // http://localhost:3123/audio/duas/1_1.mp3
    [HttpGet("/audio/duas/{fileName}")]
    public async Task<IActionResult> GetDuasAudio([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"audio/duas/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/audio/duas/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "audio/mp3");
    }
    
    // http://localhost:3123/audio/99NamesOfAllah/1.m4a
    [HttpGet("/audio/99NamesOfAllah/{fileName}")]
    public async Task<IActionResult> GetNamesOfAllahAudio([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"audio/99NamesOfAllah/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/audio/99NamesOfAllah/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "audio/mp3");
    }
    
    // http://localhost:3123/audio/wbw/001_001_001.m4a
    [HttpGet("/audio/wbw/{fileName}")]
    public async Task<IActionResult> GetWbwAudio([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"audio/wbw/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/audio/wbw/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "audio/mp3");
    }
    
        // http://localhost:3123/audio/memorise/0A5639E55EA4CF708D349C6FC8D95BE7CED289AFC0875F5F306CA3D3ECDA3CE9.mp3
        [HttpGet("/audio/memorise/{fileName}")]
        public async Task<IActionResult> GetMemoriseAudio([FromRoute] string fileName)
        {
            var sw = Stopwatch.StartNew();
    
            var data = await _staticFileService.GetFileContentBytesAsync($"audio/memorise/{fileName}");
            
            if (data is null)
            {
                return NotFound();
            }
    
            _logger.LogInformation("/audio/memorise/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
            
            return File(data, "audio/mp3");
        }
}