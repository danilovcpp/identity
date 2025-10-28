using Identity.Domain.Entities;

namespace Identity.Api.Abstractions;

public interface IPasswordResetEmailService
{
    Task SendPasswordResetLink(ApplicationUser user);
}
