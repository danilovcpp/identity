using Identity.Application.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/reset-password")]
public class ResetPasswordController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromServices] ISender sender,
        [FromBody] ResetPasswordRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}
