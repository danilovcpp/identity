using System.Security.Claims;
using Identity.Application.Accounts.Commands.DeleteAvatar;
using Identity.Application.Accounts.Commands.UploadAvatar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/avatar")]
[Authorize]
public class AvatarController : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(
        IFormFile file)
    {
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (string.IsNullOrEmpty(userId))
        // {
        //     return Unauthorized();
        // }
        //
        // if (file is null || file.Length == 0)
        // {
        //     return BadRequest(new { message = "No file uploaded." });
        // }
        //
        // await using var stream = file.OpenReadStream();
        //
        // var request = new UploadAvatarCommand(
        //     userId,
        //     stream,
        //     file.FileName,
        //     file.ContentType);
        //
        // var response = await sender.Send(request);
        //
        // return Ok(response);
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAvatar()
    {
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (string.IsNullOrEmpty(userId))
        // {
        //     return Unauthorized();
        // }
        //
        // var request = new DeleteAvatarCommand(userId);
        // await sender.Send(request);

        return NoContent();
    }
}
