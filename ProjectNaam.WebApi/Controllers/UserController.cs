using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize] // ✅ Alleen ingelogde gebruikers mogen deze API gebruiken
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { UserId = userId, Email = userEmail });
    }
}
