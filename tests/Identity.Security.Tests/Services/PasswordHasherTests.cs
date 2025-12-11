using Identity.Security.Abstractions;
using Identity.Security.Services;

namespace Identity.Security.Tests.Services;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHash_ForDifferentUsers()
    {
        var hasher = new PasswordHasher<DummyUser>();

        var hash1 = hasher.HashPassword(new DummyUser(), "Password123");
        var hash2 = hasher.HashPassword(new DummyUser(), "Password123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyHashedPassword_ReturnsSuccess_ForCorrectPassword()
    {
        var user = new DummyUser();
        var hasher = new PasswordHasher<DummyUser>();
        user.PasswordHash = hasher.HashPassword(user, "Secret!123");

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash!, "Secret!123");

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyHashedPassword_ReturnsFailed_ForIncorrectPassword()
    {
        var user = new DummyUser();
        var hasher = new PasswordHasher<DummyUser>();
        user.PasswordHash = hasher.HashPassword(user, "Secret!123");

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash!, "WrongPassword");

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    private class DummyUser
    {
        public string? PasswordHash { get; set; }
    }
}