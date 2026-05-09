namespace Identity.Domain.Users;

public enum UserStatus
{
    PendingVerification = 1,
    Active = 2,
    Blocked = 3,
    Disabled = 4,
    Deleted = 5
}