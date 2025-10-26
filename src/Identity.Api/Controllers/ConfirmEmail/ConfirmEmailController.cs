using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.ConfirmEmail;

[ApiController]
[Route("api/confirm-email")]
public class ConfirmEmailController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(
        [FromServices] ConfirmEmailRequestHandler requestHandler,
        [FromQuery] ConfirmEmailRequest request)
    {
        var response = await requestHandler.HandleAsync(request);
        return Ok(response);
    }
}