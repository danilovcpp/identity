using Identity.Security.Core;

namespace Identity.Security.Abstractions;

public interface IPasswordValidator<TUser>
{
    Task<IdentityResult> ValidateAsync(TUser user, string password);
}
