using Identity.Api.Abstractions;
using Identity.Application.Abstractions;
using Identity.Application.Models.Options;
using Identity.Application.Refresh.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Refresh;

public class RefreshRequestHandler(
    IApplicationDbContext context,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<RefreshRequest, RefreshResponse>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<RefreshResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);

        var userRefreshToken = await context.UserRefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

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