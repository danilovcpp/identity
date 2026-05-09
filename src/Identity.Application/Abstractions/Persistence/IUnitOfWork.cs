using System.Data;

namespace Identity.Application.Abstractions.Persistence;

/// <summary>
/// Commits the current EF Core change set as one transaction. Repositories track
/// changes via the shared DbContext; the unit of work is what actually persists them.
///
/// Handlers call SaveChangesAsync exactly once at the end of the use case.
/// Within the same operation, multiple repositories share the same context and
/// participate in the same transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);

    /// <summary>
    /// Run an operation inside an explicit transaction with the given isolation level.
    /// Used for operations that need stronger isolation than the default ReadCommitted
    /// (notably refresh-token rotation, which needs SELECT FOR UPDATE semantics).
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        IsolationLevel isolation,
        Func<CancellationToken, Task<T>> action,
        CancellationToken ct);
}