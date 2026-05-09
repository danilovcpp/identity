using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Integrations;
using Identity.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services;

public class EmailConfirmationService(
    IEmailSender emailSender,
    IConfirmationLinkGenerator confirmationLinkGenerator,
    IConfirmationEmailBuilder confirmationEmailBuilder) : IEmailConfirmationService
{
    private const string ConfirmationSubject = "Подтверждение регистрации";

    public async Task SendConfirmationLink(User user)
    {
        // var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        // var confirmationLink = confirmationLinkGenerator.CreateConfirmationLink(user.Id, confirmationToken);
        // var emailBody = confirmationEmailBuilder.CreateEmailBody(confirmationLink);
        //
        // await emailSender.SendEmailAsync(
        //     user.Email,
        //     ConfirmationSubject,
        //     emailBody);
        throw new NotImplementedException();
    }
}