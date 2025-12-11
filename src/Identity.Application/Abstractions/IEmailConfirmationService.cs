using Identity.Domain.Entities;

namespace Identity.Application.Abstractions;

public interface IEmailConfirmationService
{
    Task SendConfirmationLink(ApplicationUser user);
}