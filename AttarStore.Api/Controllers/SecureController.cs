// Controllers/SecureController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly() => Ok("Welcome Admin!");

    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public IActionResult UserOnly() => Ok("Welcome User!");

    [HttpGet("client")]
    [Authorize(Roles = "Client")]
    public IActionResult ClientOnly() => Ok("Welcome Client!");
}
