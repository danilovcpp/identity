using Identity.Api.Core;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Login;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(
        [FromServices] IRequestHandler<LoginRequest, LoginResponse> handler,
        [FromBody] LoginRequest request)
    {
        var response = await handler.HandleAsync(request);
        return Ok(response);
    }
}