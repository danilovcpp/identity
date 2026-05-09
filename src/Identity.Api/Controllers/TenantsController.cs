using Identity.Application.Tenants.Commands.CreateTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
//[Authorize]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantCommand command,
        [FromServices] CreateTenantCommandHandler commandHandler)
    {
        var result = await commandHandler.HandleAsync(command, HttpContext.RequestAborted);
        return Ok(result.Value);
    }
}