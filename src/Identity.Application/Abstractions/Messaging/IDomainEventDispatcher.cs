using Identity.Core;

namespace Identity.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher 
{
    Task DispatchAsync(IDomainEvent @event, CancellationToken ct);
}