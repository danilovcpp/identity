namespace Identity.Security.Abstractions;

public interface IPasswordHasher
{
    string HashPassword(string password);
    PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
}

public enum PasswordVerificationResult
{
    Failed,
    Success,
    SuccessRehashNeeded
}