using Identity.Domain.Entities;

namespace Identity.Application.Abstractions;

public interface IAccessTokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}