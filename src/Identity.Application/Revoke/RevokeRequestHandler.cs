using Identity.Application.Abstractions;
using Identity.Application.Revoke.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Revoke;

public class RevokeRequestHandler(
    IApplicationDbContext context,
    IRefreshTokenService refreshTokenService) : IRequestHandler<RevokeRequest, RevokeResponse>
{
    public async Task<RevokeResponse> Handle(RevokeRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);

        var userRefreshToken = await context.UserRefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

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