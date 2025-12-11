using Identity.Api.Controllers.Login;
using Identity.Application.Abstractions;
using Identity.Application.Login.Exceptions;
using Identity.Application.Models.Options;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Login;

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

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new UnauthorizedException();
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