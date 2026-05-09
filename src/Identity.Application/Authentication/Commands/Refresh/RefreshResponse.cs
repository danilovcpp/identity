namespace Identity.Application.Authentication.Commands.Refresh;

public record RefreshResponse(string AccessToken, string RefreshToken, int ExpiresIn);