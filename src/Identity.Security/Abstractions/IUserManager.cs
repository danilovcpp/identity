using Identity.Security.Core;

namespace Identity.Security.Abstractions;

public interface IUserManager<in TUser>
    where TUser : class, IUser
{
    Task<IdentityResult> CreateUser(TUser user, string password);
}
