using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Register;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromServices] RegisterRequestHandler handler,
        [FromBody] RegisterRequest request)
    {
        var response = await handler.HandleAsync(request);
        return Ok(response);
    }
}