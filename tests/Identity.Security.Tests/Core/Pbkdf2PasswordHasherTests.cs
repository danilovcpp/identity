using Identity.Security.Services;

namespace Identity.Security.Tests.Core;

public class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void HashPassword_GeneratesDifferentHashes()
    {
        const string password = "password";

        var sut = new Pbkdf2PasswordHasher();

        var hash1 = sut.HashPassword(password);
        var hash2 = sut.HashPassword(password);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_VerifyHashedPassword()
    {
        const string password = "password";

        var sut = new Pbkdf2PasswordHasher();

        var hash = sut.HashPassword(password);
        var result = sut.VerifyHashedPassword(hash, password);

        Assert.True(result);
    }
}