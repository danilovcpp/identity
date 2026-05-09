using Identity.Application.ResetPassword.Exceptions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.ResetPassword;

public class ResetPasswordRequestHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordRequest, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new UserNotFoundException();
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new InvalidPasswordResetTokenException();
        }

        return new ResetPasswordResponse();
    }
}
