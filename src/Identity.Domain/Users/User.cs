using Identity.Domain.Roles;

namespace Identity.Domain.Users;

public sealed class User
{
    public Guid Id { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? DisplayName { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTimeOffset? EmailVerifiedAt { get; private set; }
    public DateTimeOffset? PhoneVerifiedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles;

    private User() { }

    public static User Create(string email, string? displayName, DateTimeOffset now)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            DisplayName = displayName,
            Status = UserStatus.PendingVerification,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Activate(DateTimeOffset now)
    {
        Status = UserStatus.Active;
        EmailVerifiedAt = now;
        UpdatedAt = now;
    }

    public void Block(DateTimeOffset now)
    {
        Status = UserStatus.Blocked;
        UpdatedAt = now;
    }

    public void Unblock(DateTimeOffset now)
    {
        Status = UserStatus.Active;
        UpdatedAt = now;
    }
}