using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Common;
using Identity.Application.Models.Options;
using Identity.Core;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Commands.Login;

public class LoginCommandHandler(
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) : ICommandHandler<LoginCommand, LoginResponse>
{
    private const int RefreshTokenExpireTimeInDays = 7;

    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<LoginResponse>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        // var user = await userManager.FindByEmailAsync(command.Email) ?? throw new UnauthorizedException();
        // if (!await userManager.IsEmailConfirmedAsync(user))
        // {
        //     throw new EmailNotConfirmedException();
        // }
        //
        // var result = await signInManager.CheckPasswordSignInAsync(user, command.Password, false);
        // if (!result.Succeeded)
        // {
        //     throw new UnauthorizedException();
        // }
        //
        // var accessToken = await accessTokenService.GenerateAccessTokenAsync(user);
        // var refreshToken = refreshTokenService.GenerateRefreshToken();
        // var tokenHash = refreshTokenService.HashToken(refreshToken);
        //
        // var userRefreshToken = new UserRefreshToken
        // {
        //     UserId = user.Id,
        //     TokenHash = tokenHash,
        //     ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpireTimeInDays),
        //     CreatedAt = DateTime.UtcNow
        // };
        //
        // context.UserRefreshTokens.Add(userRefreshToken);
        // await context.SaveChangesAsync(CancellationToken.None);

        return new LoginResponse(
            "accessToken",
            "refreshToken",
            _jwtOptions.AccessTokenLifetimeMinutes * 60); // in seconds
    }
}