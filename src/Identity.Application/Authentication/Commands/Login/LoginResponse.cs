namespace Identity.Application.Authentication.Commands.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);