using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Integrations;
using Identity.Domain.Users;

namespace Identity.Api.Services;

public class PasswordResetEmailService(
    IEmailSender emailSender,
    IPasswordResetLinkGenerator passwordResetLinkGenerator,
    IPasswordResetEmailBuilder passwordResetEmailBuilder) : IPasswordResetEmailService
{
    private const string PasswordResetSubject = "Сброс пароля";

    public async Task SendPasswordResetLink(User user)
    {
        // var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        // var resetLink = passwordResetLinkGenerator.CreatePasswordResetLink(user.Email!, resetToken);
        // var emailBody = passwordResetEmailBuilder.CreateEmailBody(resetLink);
        //
        // await emailSender.SendEmailAsync(
        //     user.Email!,
        //     PasswordResetSubject,
        //     emailBody);
        throw new NotImplementedException();
    }
}
