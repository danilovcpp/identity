namespace Identity.Api.Controllers.Refresh;

public record RefreshResponse(string AccessToken, string RefreshToken, int ExpiresIn);