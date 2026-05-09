using Identity.Application.Authentication.Commands.ChangePassword;
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
        [FromBody] ChangePasswordCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}
