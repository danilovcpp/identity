using Identity.Application.Abstractions.Messaging;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common;
using Identity.Core;
using Identity.Domain.Tenants;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Tenants.Commands.CreateTenant;

public sealed class CreateTenantCommandHandler(
    ILogger<CreateTenantCommandHandler> logger,
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork,
    IOutboxWriter outbox,
    IClock clock) : ICommandHandler<CreateTenantCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateTenantCommand command, CancellationToken ct)
    {
        // 1. Parse value objects. Domain says no on bad input.
        var slugResult = Slug.Create(command.Slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        // 2. Cross-aggregate uniqueness check. This is done OUTSIDE the aggregate
        //    because uniqueness spans many aggregates (the whole tenants table).
        //    Race condition: two concurrent CreateTenant calls with the same slug
        //    both pass this check, then both try to insert. The DB unique index
        //    catches this — see catch block below for the recovery path.
        if (await tenantRepository.SlugExistsAsync(slugResult.Value, ct))
        {
            return TenantErrors.SlugAlreadyTaken(command.Slug);
        }

        // 3. Construct the aggregate via the domain factory.
        var tenantResult = Tenant.Create(slugResult.Value, command.Name, clock);
        if (tenantResult.IsFailure) return tenantResult.Error;

        var tenant = tenantResult.Value;

        // 4. Track for insertion.
        tenantRepository.Add(tenant);

        // 5. Enqueue any integration messages in the same transaction.
        outbox.Enqueue(
            messageType: "TenantCreated",
            payload: new { TenantId = tenant.Id.Value, Slug = tenant.Slug.Value, tenant.Name });

        // 6. Commit. If the unique-index check from step 2 lost a race, this throws
        //    a DbUpdateException with a Postgres unique-violation; we map it to a
        //    domain error rather than letting it bubble.
        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException ex) when (ex.ConstraintName == "uq_tenants_slug")
        {
            logger.LogInformation("Tenant slug uniqueness race lost for {Slug}", command.Slug);
            return TenantErrors.SlugAlreadyTaken(command.Slug);
        }

        logger.LogInformation("Tenant {TenantId} created with slug {Slug}", tenant.Id, tenant.Slug);
        return tenant.Id.Value;
    }
}