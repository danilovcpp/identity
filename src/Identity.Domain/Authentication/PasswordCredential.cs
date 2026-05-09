namespace Identity.Domain.Authentication;

public sealed class PasswordCredential
{
    public Guid UserId { get; private set; }
    public string PasswordHash { get; private set; } = default!;
    public DateTimeOffset PasswordChangedAt { get; private set; }
    public bool MustChangePassword { get; private set; }
    public int FailedAttempts { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private PasswordCredential() { }

    public static PasswordCredential Create(Guid userId, string passwordHash, DateTimeOffset now)
    {
        return new PasswordCredential
        {
            UserId = userId,
            PasswordHash = passwordHash,
            PasswordChangedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void ChangePassword(string passwordHash, DateTimeOffset now)
    {
        PasswordHash = passwordHash;
        PasswordChangedAt = now;
        MustChangePassword = false;
        FailedAttempts = 0;
        LockedUntil = null;
        UpdatedAt = now;
    }

    public void RegisterFailedAttempt(DateTimeOffset now, int maxAttempts, TimeSpan lockDuration)
    {
        FailedAttempts++;
        if (FailedAttempts >= maxAttempts)
            LockedUntil = now.Add(lockDuration);

        UpdatedAt = now;
    }

    public void ResetFailures(DateTimeOffset now)
    {
        FailedAttempts = 0;
        LockedUntil = null;
        UpdatedAt = now;
    }
}