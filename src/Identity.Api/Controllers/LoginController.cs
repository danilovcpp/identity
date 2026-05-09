using Identity.Application.Authentication.Commands.Login;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}