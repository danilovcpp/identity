using Identity.Security.Core;

namespace Identity.Security.Tests.Core;

public class IdentityResultTests
{
    [Fact]
    public void Success_CreatesSucceededResult()
    {
        var result = IdentityResult.Success();

        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failed_CreatesFailedResult_WithErrors()
    {
        var result = IdentityResult.Failed("err1", "err2");

        Assert.False(result.Succeeded);
        Assert.Equal(new[] { "err1", "err2" }, result.Errors);
    }
}