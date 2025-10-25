using Identity.Api.Abstractions;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services;

public class EmailConfirmationService(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IConfirmationLinkGenerator confirmationLinkGenerator,
    IConfirmationEmailBuilder confirmationEmailBuilder) : IEmailConfirmationService
{
    public async Task SendConfirmationLink(ApplicationUser user)
    {
        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = confirmationLinkGenerator.CreateConfirmationLink(user.Id, confirmationToken);
        var emailBody = confirmationEmailBuilder.CreateEmailBody(confirmationLink);

        await emailSender.SendEmailAsync(
            user.Email,
            "Подтверждение регистрации",
            emailBody);
    }
}