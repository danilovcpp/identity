using Identity.Api.Abstractions;
using Identity.Api.Controllers.Login.Exceptions;
using Identity.Api.Core;
using Identity.Api.Entities;
using Identity.Api.Models.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Api.Controllers.Login;

public class LoginRequestHandler(
    IApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<LoginRequest, LoginResponse>
{
    private const int RefreshTokenExpireTimeInDays = 7;

    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<LoginResponse> HandleAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedException();
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            throw new EmailNotConfirmedException();
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedException();
        }

        var accessToken = await accessTokenService.GenerateAccessTokenAsync(user);
        var refreshToken = refreshTokenService.GenerateRefreshToken();
        var tokenHash = refreshTokenService.HashToken(refreshToken);

        var userRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpireTimeInDays),
            CreatedAt = DateTime.UtcNow
        };

        context.UserRefreshTokens.Add(userRefreshToken);
        await context.SaveChangesAsync(CancellationToken.None);

        return new LoginResponse(
            accessToken,
            refreshToken,
            _jwtOptions.AccessTokenLifetimeMinutes * 60); // in seconds
    }
}