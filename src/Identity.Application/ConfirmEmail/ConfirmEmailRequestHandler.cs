using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.ConfirmEmail;

public class ConfirmEmailRequestHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<ConfirmEmailRequest, ConfirmEmailResponse>
{
    public async Task<ConfirmEmailResponse> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        // if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        //     return BadRequest(new { message = "UserId и token обязательны" });

        var user = await userManager.FindByIdAsync(request.UserId) ?? throw new Exception("User not found");
        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            throw new Exception("Error while confirming email");
        }

        return new ConfirmEmailResponse();
    }
}