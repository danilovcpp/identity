using System.Security.Cryptography;
using Identity.Security.Abstractions;

namespace Identity.Security.Services;

public class PasswordHasher<TUser> : IPasswordHasher<TUser>
{
    private const int Iterations = 310000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public string HashPassword(TUser user, string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            KeySize);

        return Convert.ToBase64String(
            salt.Concat(key).ToArray());
    }

    public PasswordVerificationResult VerifyHashedPassword(
        TUser user,
        string hashedPassword,
        string providedPassword)
    {
        var bytes = Convert.FromBase64String(hashedPassword);
        var salt = bytes[..SaltSize];
        var savedKey = bytes[SaltSize..];

        var key = Rfc2898DeriveBytes.Pbkdf2(
            providedPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            KeySize);

        return CryptographicOperations.FixedTimeEquals(key, savedKey)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}