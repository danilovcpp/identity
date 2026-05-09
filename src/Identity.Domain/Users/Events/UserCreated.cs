using Identity.Core;

namespace Identity.Domain.Users.Events;

public sealed record UserCreated(
    UserId UserId,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();
}