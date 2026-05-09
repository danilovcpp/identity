using Identity.Application.Authentication.Commands.Refresh;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/refresh")]
public class RefreshController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshCommand command)
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}