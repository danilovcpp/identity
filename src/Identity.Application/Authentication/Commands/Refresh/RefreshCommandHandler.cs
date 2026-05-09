using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Common;
using Identity.Application.Models.Options;
using Identity.Core;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Commands.Refresh;

public class RefreshCommandHandler(
    IApplicationDbContext context,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) : ICommandHandler<RefreshCommand, RefreshResponse>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = refreshTokenService.HashToken(command.RefreshToken);

        // var userRefreshToken = await context.UserRefreshTokens
        //     .Include(rt => rt.User)
        //     .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        // if (userRefreshToken == null || !userRefreshToken.IsActive)
        // {
        //     throw new InvalidRefreshTokenException();
        // }

        //var accessToken = await accessTokenService.GenerateAccessTokenAsync(userRefreshToken.User);

        return new RefreshResponse(
            "accessToken",
            command.RefreshToken,
            _jwtOptions.RefreshTokenLifetimeDays * 24 * 3600); // in seconds
    }
}