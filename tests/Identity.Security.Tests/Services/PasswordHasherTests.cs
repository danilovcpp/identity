using Identity.Security.Abstractions;
using Identity.Security.Services;

namespace Identity.Security.Tests.Services;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHash_ForSamePassword()
    {
        var hasher = new PasswordHasher();

        var hash1 = hasher.HashPassword("Password123");
        var hash2 = hasher.HashPassword("Password123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyHashedPassword_ReturnsSuccess_ForCorrectPassword()
    {
        var hasher = new PasswordHasher();
        var passwordHash = hasher.HashPassword("Secret!123");

        var result = hasher.VerifyHashedPassword(passwordHash, "Secret!123");

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyHashedPassword_ReturnsFailed_ForIncorrectPassword()
    {
        var hasher = new PasswordHasher();
        var passwordHash = hasher.HashPassword("Secret!123");

        var result = hasher.VerifyHashedPassword(passwordHash, "WrongPassword");

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }
}