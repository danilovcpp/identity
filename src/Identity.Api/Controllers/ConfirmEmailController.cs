using Identity.Application.Authentication.Commands.ConfirmEmail;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/confirm-email")]
public class ConfirmEmailController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] ConfirmEmailCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}