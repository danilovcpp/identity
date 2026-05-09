using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/revoke")]
public class RevokeController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Revoke()
    {
        //return Ok(response);
        throw new NotImplementedException();
    }
}