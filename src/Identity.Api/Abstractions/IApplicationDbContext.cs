using Identity.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Abstractions;

public interface IApplicationDbContext
{
    DbSet<UserRefreshToken> UserRefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}