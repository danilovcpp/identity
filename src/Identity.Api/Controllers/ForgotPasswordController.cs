using Identity.Application.ForgotPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/forgot-password")]
public class ForgotPasswordController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
        [FromServices] ISender sender,
        [FromBody] ForgotPasswordRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}
