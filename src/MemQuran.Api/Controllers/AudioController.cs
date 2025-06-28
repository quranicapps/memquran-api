using System.Diagnostics;
using MemQuran.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AudioController(IStaticFileService staticFileService, ILogger<AudioController> logger)
    : ControllerBase
{
    // https://localhost:3123/json/audio/abderahmane-eloosi-1/timings/surah/abderahmane-eloosi-1_timings_surah_1.json
    [EndpointName("GetSurahAudioJson")]
    [EndpointDescription("Get Surah audio timings in JSON format")]
    [EndpointSummary("Get Surah audio timings in JSON format")]
    [HttpGet("/json/audio/{reciterId}/timings/surah/{fileName}")]
    public async Task<IActionResult> GetSurahAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/surah/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/audio/{ReciterId}/timings/surah/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://localhost:3123/json/audio/abu-bakr-ash-shaatree-1/timings/juz/abu-bakr-ash-shaatree-1_timings_juz_1.json
    [EndpointName("GetJuzAudioJson")]
    [EndpointDescription("Get Juz audio timings in JSON format")]
    [EndpointSummary("Get Juz audio timings in JSON format")]
    [HttpGet("/json/audio/{reciterId}/timings/juz/{fileName}")]
    public async Task<IActionResult> GetJuzAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/juz/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/audio/{ReciterId}/timings/juz/{FileName} loaded in {Elapsed} ms", reciterId,
            fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/page/khalifah-al-tunaiji-161-3_timings_page_604.json
    [EndpointName("GetPageAudioJson")]
    [EndpointDescription("Get Page audio timings in JSON format")]
    [EndpointSummary("Get Page audio timings in JSON format")]
    [HttpGet("/json/audio/{reciterId}/timings/page/{fileName}")]
    public async Task<IActionResult> GetPageAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/page/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/audio/{ReciterId}/timings/page/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/ruku/khalifah-al-tunaiji-161-3_timings_ruku_300.json
    [EndpointName("GetRukuAudioJson")]
    [EndpointDescription("Get Ruku audio timings in JSON format")]
    [EndpointSummary("Get Ruku audio timings in JSON format")]
    [HttpGet("/json/audio/{reciterId}/timings/ruku/{fileName}")]
    public async Task<IActionResult> GetRukuAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/ruku/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/audio/{ReciterId}/timings/ruku/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://127.0.0.1:3123/json/audio/khalifah-al-tunaiji-161-3/timings/maqra/khalifah-al-tunaiji-161-3_timings_maqra_300.json
    [EndpointName("GetMaqraAudioJson")]
    [EndpointDescription("Get Maqra audio timings in JSON format")]
    [EndpointSummary("Get Maqra audio timings in JSON format")]
    [HttpGet("/json/audio/{reciterId}/timings/maqra/{fileName}")]
    public async Task<IActionResult> GetMaqraAudioJson([FromRoute] string reciterId, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var text = await staticFileService.GetFileContentStringAsync($"json/audio/{reciterId}/timings/maqra/{fileName}");

        if (text is null)
        {
            return NotFound();
        }

        logger.LogInformation("/json/audio/{ReciterId}/timings/maqra/{FileName} loaded in {Elapsed} ms", reciterId, fileName, sw.Elapsed);

        return Ok(text);
    }

    // https://localhost:3123/audio/tajweed/ikhfa.mp3
    // https://localhost:3123/audio/tajweed/tajweed_must_stop_6-36.mp3
    // https://localhost:3123/audio/duas/1_1.mp3
    // https://localhost:3123/audio/namesOfAllah/1.mp3
    // https://localhost:3123/audio/wbw/001_001_001.mp3
    // https://localhost:3123/audio/memorise/0A5639E55EA4CF708D349C6FC8D95BE7CED289AFC0875F5F306CA3D3ECDA3CE9.mp3
    // https://localhost:3123/audio/common/correct.mp3
    [EndpointName("GetAudioByType")]
    [EndpointDescription("Get audio mp3 file by type and file name")]
    [EndpointSummary("Get audio mp3 file by type and file name")]
    [HttpGet("/audio/{type}/{fileName}")]
    public async Task<IActionResult> GetAudioByType([FromRoute] string type, [FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await staticFileService.GetFileContentBytesAsync($"audio/{type}/{fileName}");

        if (data is null)
        {
            return NotFound();
        }

        logger.LogInformation("/audio/{Type}/{FileName} loaded in {Elapsed} ms", type, fileName, sw.Elapsed);

        return File(data, "audio/mp3");
    }
}