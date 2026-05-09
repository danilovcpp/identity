using System.Security.Claims;
using Identity.Application.Accounts.Queries.GetProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfile(
        [FromRoute] string userId)
    {
        // var request = new GetProfileQuery(userId);
        // return Ok(response);
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //
        // if (string.IsNullOrEmpty(userId))
        // {
        //     return Unauthorized();
        // }
        //
        // var request = new GetProfileQuery(userId);
        // var response = await sender.Send(request);
        //return Ok(response);
        throw new NotImplementedException();
    }
}
