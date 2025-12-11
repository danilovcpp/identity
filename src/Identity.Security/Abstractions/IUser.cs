namespace Identity.Security.Abstractions;

public interface IUser
{
    Guid Id { get; }
    string UserName { get; set; }
    string NormalizedUserName { get; set; }
    string PasswordHash { get; set; }
    bool IsLockedOut { get; set; }
}
