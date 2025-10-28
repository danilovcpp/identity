using Identity.Domain.Entities;

namespace Identity.Api.Abstractions;

public interface IEmailConfirmationService
{
    Task SendConfirmationLink(ApplicationUser user);
}