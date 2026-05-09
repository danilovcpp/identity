using Identity.Domain.Users;

namespace Identity.Application.Abstractions;

public interface IPasswordResetEmailService
{
    Task SendPasswordResetLink(User user);
}
