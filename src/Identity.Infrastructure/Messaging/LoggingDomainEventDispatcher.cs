using Identity.Application.Abstractions.Messaging;
using Identity.Core;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Messaging;

public class LoggingDomainEventDispatcher(
    ILogger<LoggingDomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent @event, CancellationToken ct)
    {
        logger.LogInformation("Domain event: {@event}", @event);
        return Task.CompletedTask;
    }
}