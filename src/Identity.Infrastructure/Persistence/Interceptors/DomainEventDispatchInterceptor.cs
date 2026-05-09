using Identity.Application.Abstractions.Messaging;
using Identity.Core;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Identity.Infrastructure.Persistence.Interceptors;

internal sealed class DomainEventDispatchInterceptor(IDomainEventDispatcher dispatcher)
    : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData evt,
        int result,
        CancellationToken ct = default)
    {
        // Dispatch AFTER successful save so handlers see committed state.
        // Caveat: in-process handlers run outside the original transaction.
        // For events that need transactional guarantees, use the outbox instead.
        if (evt.Context is null) return result;

        var aggregates = evt.Context.ChangeTracker
            .Entries()
            .Select(e => e.Entity)
            .OfType<IHasDomainEvents>()
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
        foreach (var a in aggregates) a.ClearDomainEvents();

        foreach (var e in events)
            await dispatcher.DispatchAsync(e, ct);

        return result;
    }
}