using Identity.Security.Abstractions;

namespace Identity.Security.Domain;

public class IdentityUser : IUser
{
    public Guid Id { get; }
    public string UserName { get; set; }
    public string NormalizedUserName { get; set; }
    public string PasswordHash { get; set; }
    public bool IsLockedOut { get; set; }
}