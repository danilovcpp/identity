using Identity.Application.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(
        [FromServices] ISender sender,
        [FromBody] LoginRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}