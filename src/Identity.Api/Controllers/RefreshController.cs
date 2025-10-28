using Identity.Application.Refresh;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/refresh")]
public class RefreshController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Refresh(
        [FromServices] ISender sender,
        [FromBody] RefreshRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}