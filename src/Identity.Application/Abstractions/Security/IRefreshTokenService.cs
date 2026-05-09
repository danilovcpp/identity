namespace Identity.Application.Abstractions.Security;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
    string HashToken(string token);
}