using System.Security.Cryptography;
using Identity.Security.Abstractions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Identity.Security.Services;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA512;
    private const byte FormatMarker = 0x01;
    private const int IterationCount = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public string HashPassword(string password)
    {
        var salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        var subkey = KeyDerivation.Pbkdf2(
            password,
            salt,
            Prf,
            IterationCount,
            KeySize);

        var outputBytes = new byte[13 + salt.Length + subkey.Length];
        outputBytes[0] = FormatMarker;

        WriteNetworkByteOrder(outputBytes, 1, (uint)Prf);
        WriteNetworkByteOrder(outputBytes, 5, (uint)IterationCount);
        WriteNetworkByteOrder(outputBytes, 9, (uint)SaltSize);

        Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 13 + SaltSize, subkey.Length);

        return Convert.ToBase64String(outputBytes);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        try
        {
            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            var prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPasswordBytes, 1);
            var iterationCount = (int)ReadNetworkByteOrder(hashedPasswordBytes, 5);
            var saltLength = (int)ReadNetworkByteOrder(hashedPasswordBytes, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }

            var salt = hashedPasswordBytes.AsSpan(13, saltLength).ToArray();

            // Read the subkey (the rest of the payload): must be >= 128 bits
            var subkeyLength = hashedPasswordBytes.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }

            var expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterationCount, subkeyLength);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
        catch
        {
            // This should never occur except in the case of a malformed payload, where
            // we might go off the end of the array. Regardless, a malformed payload
            // implies verification failed.
            return false;
        }
    }

    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
        return ((uint)(buffer[offset + 0]) << 24)
               | ((uint)(buffer[offset + 1]) << 16)
               | ((uint)(buffer[offset + 2]) << 8)
               | ((uint)(buffer[offset + 3]));
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }
}