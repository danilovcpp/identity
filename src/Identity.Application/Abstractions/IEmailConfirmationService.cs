using Identity.Domain.Users;

namespace Identity.Application.Abstractions;

public interface IEmailConfirmationService
{
    Task SendConfirmationLink(User user);
}