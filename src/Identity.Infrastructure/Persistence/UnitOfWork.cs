using System.Data;
using Identity.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infrastructure.Persistence;

internal sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct)
        => context.SaveChangesAsync(ct);

    public async Task<T> ExecuteInTransactionAsync<T>(
        IsolationLevel isolation,
        Func<CancellationToken, Task<T>> action,
        CancellationToken ct)
    {
        // If we're already inside a transaction (e.g. nested handler), don't start a new one.
        if (context.Database.CurrentTransaction is not null)
            return await action(ct);

        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(isolation, ct);
        var result = await action(ct);
        await transaction.CommitAsync(ct);
        return result;
    }
}