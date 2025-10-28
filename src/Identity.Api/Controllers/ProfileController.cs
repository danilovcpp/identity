using System.Security.Claims;
using Identity.Application.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfile(
        [FromServices] ISender sender,
        [FromRoute] string userId)
    {
        var request = new GetProfileRequest(userId);
        var response = await sender.Send(request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile([FromServices] ISender sender)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var request = new GetProfileRequest(userId);
        var response = await sender.Send(request);
        return Ok(response);
    }
}
