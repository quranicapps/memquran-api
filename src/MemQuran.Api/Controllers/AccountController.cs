using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

public class AccountController : Controller
{
    // https://localhost:3123/user/{userId} 
    [Authorize]
    [HttpGet("/user/{userId}")]
    public IActionResult GetUser(string userId)
    {
        return Ok(userId);
    }
}