using Identity.Api.Core;
using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Controllers.ConfirmEmail;

public class ConfirmEmailRequestHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<ConfirmEmailRequest, ConfirmEmailResponse>
{
    public async Task<ConfirmEmailResponse> HandleAsync(ConfirmEmailRequest request)
    {
        // if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        //     return BadRequest(new { message = "UserId и token обязательны" });

        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            //return BadRequest(new { message = "Ошибка подтверждения email", errors = result.Errors });
            throw new Exception("Error while confirming email");
        }

        return new ConfirmEmailResponse();
    }
}