using Identity.Core;

namespace Identity.Domain.Tenants.Events;

public sealed record TenantCreated(
    TenantId TenantId,
    Slug Slug,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();
}

public sealed record TenantSuspended(
    TenantId TenantId,
    DateTimeOffset OccurredAt,
    string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();
}

public sealed record TenantReactivated(
    TenantId TenantId,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();
}

public sealed record TenantDeleted(
    TenantId TenantId,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();
}
