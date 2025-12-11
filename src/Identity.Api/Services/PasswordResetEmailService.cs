using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services;

public class PasswordResetEmailService(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IPasswordResetLinkGenerator passwordResetLinkGenerator,
    IPasswordResetEmailBuilder passwordResetEmailBuilder) : IPasswordResetEmailService
{
    private const string PasswordResetSubject = "Сброс пароля";

    public async Task SendPasswordResetLink(ApplicationUser user)
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = passwordResetLinkGenerator.CreatePasswordResetLink(user.Email!, resetToken);
        var emailBody = passwordResetEmailBuilder.CreateEmailBody(resetLink);

        await emailSender.SendEmailAsync(
            user.Email!,
            PasswordResetSubject,
            emailBody);
    }
}
