using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuranApi.Contracts;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IStaticFileService _staticFileService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(IStaticFileService staticFileService, ILogger<ImagesController> logger)
    {
        _staticFileService = staticFileService;
        _logger = logger;
    }
    
    // http://localhost:3123/images/tajweed/1/1/1.png
    [HttpGet("/images/{imageType}/{surahNumber}/{verseNumber}/{fileName}")]
    public async Task<IActionResult> GetTajweedWord([FromRoute] string imageType, string surahNumber, string verseNumber, string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"images/{imageType}/{surahNumber}/{verseNumber}/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/images/{ImageType}/{SurahNumber}/{VerseNumber}/{FileName} loaded in {Elapsed} ms", imageType, surahNumber, verseNumber, fileName, sw.Elapsed);
        
        return File(data, "image/png");
    }
    
    // http://localhost:3123/images/tajweed/1.png
    [HttpGet("/images/tajweed/{fileName}")]
    public async Task<IActionResult> GetTajweedNumber([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"images/tajweed/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/images/tajweed/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "image/png");
    }
    
    // Todo remove once changed web to point to e.g. http://localhost:3123/images/tajweed/1.png
    // http://localhost:3123/images/common/numbers/1.png
    [HttpGet("/images/common/numbers/{fileName}")]
    public async Task<IActionResult> GetTajweedCommonNumber([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"images/common/numbers/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/images/common/numbers/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "image/png");
    }
    
    // http://localhost:3123/images/reciters/abdallah-al-matroud-1.png
    [HttpGet("/images/reciters/{fileName}")]
    public async Task<IActionResult> GetReciter([FromRoute] string fileName)
    {
        var sw = Stopwatch.StartNew();

        var data = await _staticFileService.GetFileContentBytesAsync($"images/reciters/{fileName}");
        
        if (data is null)
        {
            return NotFound();
        }

        _logger.LogInformation("/images/reciters/{FileName} loaded in {Elapsed} ms", fileName, sw.Elapsed);
        
        return File(data, "image/png");
    }
}