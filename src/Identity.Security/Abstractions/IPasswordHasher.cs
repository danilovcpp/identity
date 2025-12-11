namespace Identity.Security.Abstractions;

public interface IPasswordHasher<TUser>
{
    string HashPassword(TUser user, string password);
    PasswordVerificationResult VerifyHashedPassword(
        TUser user,
        string hashedPassword,
        string providedPassword);
}

public enum PasswordVerificationResult
{
    Failed,
    Success,
    SuccessRehashNeeded
}