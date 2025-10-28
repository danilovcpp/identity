using Identity.Application.ConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/confirm-email")]
public class ConfirmEmailController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(
        ISender sender,
        [FromQuery] ConfirmEmailRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}