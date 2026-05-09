using Identity.Application.Abstractions;
using Identity.Domain.Users;

namespace Identity.Api.Services;

public class LogConfirmationService(
    ILogger<LogConfirmationService> logger,
    IConfirmationLinkGenerator confirmationLinkGenerator) : IEmailConfirmationService
{
    public async Task SendConfirmationLink(User user)
    {
        // var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        // var confirmationLink = confirmationLinkGenerator.CreateConfirmationLink(user.Id, confirmationToken);
        //
        // logger.LogWarning("Confirmation link: {ConfirmationLink}", confirmationLink);
        throw new NotImplementedException();
    }
}