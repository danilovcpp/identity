namespace Identity.Api.Abstractions;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
    string HashToken(string token);
}