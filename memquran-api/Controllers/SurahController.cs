using Microsoft.AspNetCore.Mvc;

namespace QuranApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurahController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var surahsText = await System.IO.File.ReadAllTextAsync("Resources/Surahs/surahs.json");
        
        return Ok(surahsText);
    }
}