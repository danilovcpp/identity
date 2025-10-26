using Identity.Api.Core;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Refresh;

[ApiController]
[Route("api/refresh")]
public class RefreshController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Refresh(
        [FromServices] IRequestHandler<RefreshRequest, RefreshResponse> handler,
        [FromBody] RefreshRequest request)
    {
        var response = await handler.HandleAsync(request);
        return Ok(response);
    }
}