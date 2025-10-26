namespace Identity.Api.Controllers.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);