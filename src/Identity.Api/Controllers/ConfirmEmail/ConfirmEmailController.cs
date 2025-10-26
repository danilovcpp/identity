using Identity.Api.Core;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.ConfirmEmail;

[ApiController]
[Route("api/confirm-email")]
public class ConfirmEmailController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(
        [FromServices] IRequestHandler<ConfirmEmailRequest, ConfirmEmailResponse> handler,
        [FromQuery] ConfirmEmailRequest request)
    {
        var response = await handler.HandleAsync(request);
        return Ok(response);
    }
}