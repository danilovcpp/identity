using Identity.Api.Abstractions;
using Identity.Api.Controllers.Revoke.Exceptions;
using Identity.Api.Core;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Controllers.Revoke;

public class RevokeRequestHandler(
    IApplicationDbContext context,
    IRefreshTokenService refreshTokenService) : IRequestHandler<RevokeRequest, RevokeResponse>
{
    public async Task<RevokeResponse> HandleAsync(RevokeRequest request)
    {
        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);

        var userRefreshToken = await context.UserRefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (userRefreshToken == null)
        {
            throw new RefreshTokenNotFoundException();
        }

        if (userRefreshToken.IsRevoked)
        {
            throw new RefreshTokenAlreadyRevokedException();
        }

        userRefreshToken.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(CancellationToken.None);

        return new RevokeResponse();
    }
}