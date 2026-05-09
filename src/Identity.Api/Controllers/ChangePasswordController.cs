using Identity.Application.Authentication.ChangePassword;
using Identity.Application.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/change-password")]
[Authorize]
public class ChangePasswordController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ChangePassword(
        [FromServices] ISender sender,
        [FromBody] ChangePasswordRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}
