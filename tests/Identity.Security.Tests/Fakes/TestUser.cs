using Identity.Security.Abstractions;

namespace Identity.Security.Tests.Fakes;

public class TestUser : IUser
{
    public Guid Id { get; } = Guid.NewGuid();
    public string UserName { get; set; } = null!;
    public string NormalizedUserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsLockedOut { get; set; }
}