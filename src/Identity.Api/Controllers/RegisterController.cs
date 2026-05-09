using Identity.Application.Authentication.Commands.Register;
using Identity.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        [FromServices] ICommandHandler<RegisterCommand, RegisterResponse> handler)
    {
        var result = await handler.HandleAsync(command, HttpContext.RequestAborted);
        return Ok(result.Value);
    }
}