using Identity.Security.Core;

namespace Identity.Security;

public interface IPasswordValidator<TUser>
{
    Task<IdentityResult> ValidateAsync(TUser user, string password);
}
