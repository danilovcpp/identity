using Identity.Application.Authentication.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}