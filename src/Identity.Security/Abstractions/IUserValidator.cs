using Identity.Security.Core;

namespace Identity.Security.Abstractions;

public interface IUserValidator<TUser>
{
    Task<IdentityResult> ValidateAsync(TUser user);
}
