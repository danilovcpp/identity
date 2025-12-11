using Identity.Domain.Entities;

namespace Identity.Application.Abstractions;

public interface IPasswordResetEmailService
{
    Task SendPasswordResetLink(ApplicationUser user);
}
