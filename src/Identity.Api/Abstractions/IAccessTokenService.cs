using Identity.Api.Entities;
using Identity.Api.Models;

namespace Identity.Api.Abstractions;

public interface IAccessTokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}