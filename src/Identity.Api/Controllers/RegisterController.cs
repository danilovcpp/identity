using Identity.Application.Authentication.Register;
using Identity.Application.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/register")]
public class RegisterController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromServices] ISender sender,
        [FromBody] RegisterRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}