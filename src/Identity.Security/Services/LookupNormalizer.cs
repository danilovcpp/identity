using Identity.Security.Abstractions;

namespace Identity.Security.Services;

public sealed class LookupNormalizer : ILookupNormalizer
{
    public string Normalize(string key)
        => key?.Trim().ToUpperInvariant() ?? string.Empty;
}