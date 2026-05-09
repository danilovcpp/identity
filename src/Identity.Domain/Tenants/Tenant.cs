using Identity.Core;
using Identity.Domain.Tenants.Events;

namespace Identity.Domain.Tenants;

public class Tenant : AggregateRoot<TenantId>, IHasDomainEvents
{
    private Tenant(
        TenantId id,
        Slug slug,
        string name,
        TenantStatus status,
        DateTimeOffset createdAt) : base(id)
    {
        Slug = slug;
        Name = name;
        Status = status;
        CreatedAt = createdAt;
    }

    public Slug Slug { get; private set; }
    public string Name { get; private set; }
    public TenantStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsActive => Status == TenantStatus.Active;

    public static Result<Tenant> Create(Slug slug, string name, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(clock);

        if (string.IsNullOrWhiteSpace(name))
        {
            return new DomainError(
                TenantErrorCodes.NameEmpty,
                TenantErrorMessages.NameEmpty);
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 200)
        {
            return new DomainError(
                TenantErrorCodes.NameTooLong,
                TenantErrorMessages.NameTooLong);
        }

        var tenant = new Tenant(
            id: TenantId.New(),
            slug: slug,
            name: trimmedName,
            status: TenantStatus.Active,
            createdAt: clock.UtcNow);

        tenant.RaiseDomainEvent(new TenantCreated(tenant.Id, slug, clock.UtcNow));

        return tenant;
    }

    public Result Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new DomainError(
                TenantErrorCodes.NameEmpty,
                TenantErrorMessages.NameEmpty);

        var trimmed = name.Trim();
        if (trimmed.Length > 200)
            return new DomainError(
                TenantErrorCodes.NameTooLong,
                TenantErrorMessages.NameTooLong);

        Name = trimmed;

        return Result.Success();
    }

    public Result Suspend(IClock clock, string? reason = null)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (Status == TenantStatus.Deleted)
            return new DomainError(
                TenantErrorCodes.Deleted,
                TenantErrorMessages.Deleted,
                DomainErrorType.Conflict);
        if (Status == TenantStatus.Suspended)
            return Result.Success();

        Status = TenantStatus.Suspended;
        SuspendedAt = clock.UtcNow;

        RaiseDomainEvent(new TenantSuspended(Id, clock.UtcNow, reason));

        return Result.Success();
    }

    public Result Reactivate(IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (Status == TenantStatus.Deleted)
            return new DomainError(
                TenantErrorCodes.Deleted,
                TenantErrorMessages.Deleted,
                DomainErrorType.Conflict);
        if (Status == TenantStatus.Active)
            return Result.Success();

        Status = TenantStatus.Active;
        SuspendedAt = null;

        RaiseDomainEvent(new TenantReactivated(Id, clock.UtcNow));

        return Result.Success();
    }

    public Result MarkDeleted(IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (Status == TenantStatus.Deleted)
            return Result.Success();

        Status = TenantStatus.Deleted;
        DeletedAt = clock.UtcNow;

        RaiseDomainEvent(new TenantDeleted(Id, clock.UtcNow));

        return Result.Success();
    }
}