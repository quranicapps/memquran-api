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

        var text = await _staticFileService.GetFileContentStringAsync(
            $"json/audio/{reciterId}/timings/surah/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/surah/{FileName} loaded in {Elapsed} ms", reciterId,
            fileName, sw.Elapsed);

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

        _logger.LogInformation("/json/audio/{ReciterId}/timings/juz/{FileName} loaded in {Elapsed} ms", reciterId,
            fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/page/khalifah-al-tunaiji-161-3_timings_page_604.json
    [HttpGet("/json/audio/{reciterId}/timings/page/{fileName}")]
    public async Task<IActionResult> GetPageAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await _staticFileService.GetFileContentStringAsync(
            $"json/audio/{reciterId}/timings/page/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/page/{FileName} loaded in {Elapsed} ms", reciterId,
            fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/ruku/khalifah-al-tunaiji-161-3_timings_ruku_300.json
    [HttpGet("/json/audio/{reciterId}/timings/ruku/{fileName}")]
    public async Task<IActionResult> GetRukuAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await _staticFileService.GetFileContentStringAsync(
            $"json/audio/{reciterId}/timings/ruku/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/ruku/{FileName} loaded in {Elapsed} ms", reciterId,
            fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/maqra/khalifah-al-tunaiji-161-3_timings_maqra_300.json
    [HttpGet("/json/audio/{reciterId}/timings/maqra/{fileName}")]
    public async Task<IActionResult> GetMaqraAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await _staticFileService.GetFileContentStringAsync(
            $"json/audio/{reciterId}/timings/maqra/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/json/audio/{ReciterId}/timings/maqra/{FileName} loaded in {Elapsed} ms", reciterId,
            fileName, sw.Elapsed);

        return Ok(text);
    }

    // http://localhost:3123/audio/tajweed/ikhfa.mp3
    // http://localhost:3123/audio/tajweed/tajweed_must_stop_6-36.mp3
    // http://localhost:3123/audio/duas/1_1.mp3
    // http://localhost:3123/audio/99NamesOfAllah/1.m4a
    // http://localhost:3123/audio/wbw/001_001_001.m4a
    // http://localhost:3123/audio/memorise/0A5639E55EA4CF708D349C6FC8D95BE7CED289AFC0875F5F306CA3D3ECDA3CE9.mp3
    // http://localhost:3123/audio/common/correct.mp3
    [HttpGet("/audio/{type}/{fileName}")]
    public async Task<IActionResult> GetTajweedAudio([FromRoute] string type, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"audio/{type}/{fileName}");

        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/audio/{Type}/{FileName} loaded in {Elapsed} ms", type, fileName, sw.Elapsed);

        return File(data, "audio/mp3");
    }
}