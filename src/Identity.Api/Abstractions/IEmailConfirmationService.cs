using Identity.Api.Models;

namespace Identity.Api.Abstractions;

public interface IEmailConfirmationService
{
    Task SendConfirmationLink(ApplicationUser user);
}