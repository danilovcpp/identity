using Identity.Domain.Users;

namespace Identity.Application.Abstractions.Security;

public interface IAccessTokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
}