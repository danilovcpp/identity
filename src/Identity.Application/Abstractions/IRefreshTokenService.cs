namespace Identity.Application.Abstractions;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
    string HashToken(string token);
}