using System.Security.Cryptography;

namespace Identity.Security.Services;

public class PasswordHasherOptions
{
    private static readonly RandomNumberGenerator DefaultRng = RandomNumberGenerator.Create();
    public int IterationCount { get; set; } = 100_000;

    // for unit testing
    internal RandomNumberGenerator Rng { get; set; } = DefaultRng;
}