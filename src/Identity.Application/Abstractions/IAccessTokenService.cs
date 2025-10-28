using Identity.Domain.Entities;

namespace Identity.Api.Abstractions;

public interface IAccessTokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}