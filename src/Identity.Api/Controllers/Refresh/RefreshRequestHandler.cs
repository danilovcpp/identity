using Identity.Api.Abstractions;
using Identity.Api.Controllers.Refresh.Exceptions;
using Identity.Api.Core;
using Identity.Api.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Api.Controllers.Refresh;

public class RefreshRequestHandler(
    IApplicationDbContext context,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<RefreshRequest, RefreshResponse>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<RefreshResponse> HandleAsync(RefreshRequest request)
    {
        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);

        var userRefreshToken = await context.UserRefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (userRefreshToken == null || !userRefreshToken.IsActive)
        {
            throw new InvalidRefreshTokenException();
        }

        var accessToken = await accessTokenService.GenerateAccessTokenAsync(userRefreshToken.User);

        return new RefreshResponse(
            accessToken,
            request.RefreshToken,
            _jwtOptions.RefreshTokenLifetimeDays * 24 * 3600); // in seconds
    }
}