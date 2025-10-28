using Identity.Application.Avatar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/avatar")]
[Authorize]
public class AvatarController : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(
        [FromServices] ISender sender,
        IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        await using var stream = file.OpenReadStream();

        var request = new UploadAvatarRequest(
            userId,
            stream,
            file.FileName,
            file.ContentType);

        var response = await sender.Send(request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAvatar([FromServices] ISender sender)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var request = new DeleteAvatarRequest(userId);
        await sender.Send(request);

        return NoContent();
    }
}
