using Identity.Api.Abstractions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services;

public class LogPasswordResetService(
    UserManager<ApplicationUser> userManager,
    IPasswordResetLinkGenerator passwordResetLinkGenerator,
    ILogger<LogPasswordResetService> logger) : IPasswordResetEmailService
{
    public async Task SendPasswordResetLink(ApplicationUser user)
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = passwordResetLinkGenerator.CreatePasswordResetLink(user.Email!, resetToken);

        logger.LogInformation(
            "Password reset link for {Email}: {ResetLink}",
            user.Email,
            resetLink);
    }
}
