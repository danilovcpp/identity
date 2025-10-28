using Identity.Application.Revoke;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/revoke")]
public class RevokeController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Revoke(
        [FromServices] ISender sender,
        [FromBody] RevokeRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}