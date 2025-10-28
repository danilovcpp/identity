using Identity.Api.Abstractions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services;

public class LogConfirmationService(
    ILogger<LogConfirmationService> logger,
    UserManager<ApplicationUser> userManager,
    IConfirmationLinkGenerator confirmationLinkGenerator) : IEmailConfirmationService
{
    public async Task SendConfirmationLink(ApplicationUser user)
    {
        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = confirmationLinkGenerator.CreateConfirmationLink(user.Id, confirmationToken);

        logger.LogWarning("Confirmation link: {ConfirmationLink}", confirmationLink);
    }
}