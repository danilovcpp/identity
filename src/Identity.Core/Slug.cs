using System.Text.RegularExpressions;

namespace Identity.Core;

/// <summary>
/// URL-safe slug. Lowercase ASCII letters, digits, and hyphen; cannot start or end with a hyphen,
/// cannot contain consecutive hyphens, length 3..63 (DNS-label compatible if we ever subdomain on it).
/// </summary>
public sealed class Slug : ValueObject
{
    public const int MinLength = 3;
    public const int MaxLength = 63;

    private static readonly Regex Pattern =
        new(@"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Result<Slug> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new DomainError("slug.empty", "Slug is required.");
        }

        var trimmed = input.Trim().ToLowerInvariant();

        if (trimmed.Length is < MinLength or > MaxLength)
        {
            return new DomainError("slug.invalid_length",
                $"Slug must be between {MinLength} and {MaxLength} characters.");
        }

        if (!Pattern.IsMatch(trimmed))
        {
            return new DomainError("slug.invalid_format",
                "Slug must contain only lowercase letters, digits, and hyphens, and cannot start or end with a hyphen.");
        }

        if (trimmed.Contains("--", StringComparison.Ordinal))
        {
            return new DomainError("slug.invalid_format", "Slug cannot contain consecutive hyphens.");
        }

        return new Slug(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
