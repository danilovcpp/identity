using Identity.Application.Authentication.Commands.ResetPassword;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/reset-password")]
public class ResetPasswordController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}
