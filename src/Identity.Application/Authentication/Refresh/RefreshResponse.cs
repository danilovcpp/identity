namespace Identity.Application.Refresh;

public record RefreshResponse(string AccessToken, string RefreshToken, int ExpiresIn);