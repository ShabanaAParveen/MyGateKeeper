using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("resource")]
public class TestController : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = "admin")]
    public IActionResult AdminOnly()
    {
        return Ok("You are ADMIN");
    }

    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public IActionResult UserOnly()
    {
        return Ok("You are USER");
    }
}