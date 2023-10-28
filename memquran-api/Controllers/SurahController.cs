using Microsoft.AspNetCore.Mvc;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    [HttpGet("{languageCode}")]
    public async Task<IActionResult> Get([FromRoute] string languageCode)
    {
        var surahsText = await System.IO.File.ReadAllTextAsync("Resources/Surahs/surahs.json");
        
        return Ok(surahsText);
    }
}