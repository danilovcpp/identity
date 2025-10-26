using Identity.Api.Core;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Register;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromServices] IRequestHandler<RegisterRequest, RegisterResponse> handler,
        [FromBody] RegisterRequest request)
    {
        var response = await handler.HandleAsync(request);
        return Ok(response);
    }
}