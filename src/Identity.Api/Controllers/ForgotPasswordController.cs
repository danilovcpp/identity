using Identity.Application.Authentication.Commands.ForgotPassword;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/forgot-password")]
public class ForgotPasswordController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}
