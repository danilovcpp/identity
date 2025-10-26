using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Revoke;

[ApiController]
[Route("api/revoke")]
public class RevokeController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Revoke(
        [FromServices] RevokeRequestHandler handler,
        [FromBody] RevokeRequest request)
    {
        var response = await handler.HandleAsync(request);
        return Ok(response);
    }
}