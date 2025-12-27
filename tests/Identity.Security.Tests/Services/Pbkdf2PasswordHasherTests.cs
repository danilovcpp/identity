using Identity.Security.Services;

namespace Identity.Security.Tests.Services;

public class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHash_ForSamePassword()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash1 = hasher.HashPassword("Password123");
        var hash2 = hasher.HashPassword("Password123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyHashedPassword_ReturnsSuccess_ForCorrectPassword()
    {
        var hasher = new Pbkdf2PasswordHasher();
        var passwordHash = hasher.HashPassword("Secret!123");

        var result = hasher.VerifyHashedPassword(passwordHash, "Secret!123");

        Assert.True(result);
    }

    [Fact]
    public void VerifyHashedPassword_ReturnsFailed_ForIncorrectPassword()
    {
        var hasher = new Pbkdf2PasswordHasher();
        var passwordHash = hasher.HashPassword("Secret!123");

        var result = hasher.VerifyHashedPassword(passwordHash, "WrongPassword");

        Assert.False(result);
    }
}