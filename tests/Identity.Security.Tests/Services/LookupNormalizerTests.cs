using Identity.Security.Services;

namespace Identity.Security.Tests.Services;

public class LookupNormalizerTests
{
    [Theory]
    [InlineData("admin", "ADMIN")]
    [InlineData("  TestUser  ", "TESTUSER")]
    [InlineData("UserName", "USERNAME")]
    [InlineData(null, "")]
    public void Normalize_ReturnsExpectedValue(string? input, string expected)
    {
        var normalizer = new LookupNormalizer();

        var result = normalizer.Normalize(input);

        Assert.Equal(expected, result);
    }
}