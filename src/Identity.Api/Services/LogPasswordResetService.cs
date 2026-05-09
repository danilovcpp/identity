using Identity.Application.Abstractions;
using Identity.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services;

public class LogPasswordResetService(
    IPasswordResetLinkGenerator passwordResetLinkGenerator,
    ILogger<LogPasswordResetService> logger) : IPasswordResetEmailService
{
    public async Task SendPasswordResetLink(User user)
    {
        // var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        // var resetLink = passwordResetLinkGenerator.CreatePasswordResetLink(user.Email!, resetToken);
        //
        // logger.LogInformation(
        //     "Password reset link for {Email}: {ResetLink}",
        //     user.Email,
        //     resetLink);
        throw new NotImplementedException();
    }
}
