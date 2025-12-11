using Identity.Security.Abstractions;
using Identity.Security.Core;
using Identity.Security.Tests.Fakes;
using Moq;

namespace Identity.Security.Tests.Core;

public class UserManagerTests
{
    [Fact]
    public async Task CreateAsync_SetsNormalizedUserName_AndHash()
    {
        var store = new Mock<IUserStore<TestUser>>();
        var hasher = new Mock<IPasswordHasher>();
        var normalizer = new Mock<ILookupNormalizer>();
        var userValidators = new[] { Mock.Of<IUserValidator<TestUser>>() };
        var passwordValidators = new[] { Mock.Of<IPasswordValidator<TestUser>>() };

        var user = new TestUser { UserName = "User" };

        hasher.Setup(h => h.HashPassword("Pass"))
            .Returns("HASH");

        normalizer.Setup(n => n.Normalize("User"))
            .Returns("USER");

        store.Setup(s => s.CreateAsync(user, CancellationToken.None))
            .Returns(Task.CompletedTask);

        var manager = new UserManager<TestUser>(
            store.Object,
            hasher.Object,
            normalizer.Object,
            userValidators,
            passwordValidators);

        var result = await manager.CreateAsync(user, "Pass");

        Assert.True(result.Succeeded);
        Assert.Equal("USER", user.NormalizedUserName);
        Assert.Equal("HASH", user.PasswordHash);
    }

    [Fact]
    public async Task FindByNameAsync_NormalizesName_BeforeSearch()
    {
        var store = new Mock<IUserStore<TestUser>>();
        var hasher = Mock.Of<IPasswordHasher>();
        var normalizer = new Mock<ILookupNormalizer>();

        normalizer.Setup(x => x.Normalize("admin"))
            .Returns("ADMIN");

        var manager = new UserManager<TestUser>(
            store.Object,
            hasher,
            normalizer.Object,
            Array.Empty<IUserValidator<TestUser>>(),
            Array.Empty<IPasswordValidator<TestUser>>());

        await manager.FindByNameAsync("admin");

        store.Verify(x => x.FindByNameAsync("ADMIN", CancellationToken.None), Times.Once);
    }

    [Fact]
    public void CheckPassword_DelegatesToHasher()
    {
        var user = new TestUser { PasswordHash = "HASH" };
        var hasher = new Mock<IPasswordHasher>();

        hasher.Setup(h => h.VerifyHashedPassword("HASH", "pass"))
            .Returns(PasswordVerificationResult.Success);

        var manager = new UserManager<TestUser>(
            Mock.Of<IUserStore<TestUser>>(),
            hasher.Object,
            Mock.Of<ILookupNormalizer>(),
            Array.Empty<IUserValidator<TestUser>>(),
            Array.Empty<IPasswordValidator<TestUser>>());

        var result = manager.CheckPassword(user, "pass");

        Assert.Equal(PasswordVerificationResult.Success, result);
    }
}