using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common;
using Identity.Core;
using Identity.Domain.Tenants;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Tenants.Commands.SuspendTenant;

public sealed class SuspendTenantCommandHandler(
    ILogger<SuspendTenantCommandHandler> logger,
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<SuspendTenantCommand>
{
    public async Task<Result<Unit>> HandleAsync(SuspendTenantCommand command, CancellationToken ct)
    {
        var tenantId = new TenantId(command.TenantId);
        var tenant = await tenantRepository.GetByIdAsync(tenantId, ct);

        if (tenant == null)
        {
            logger.LogInformation("Tenant with id {id} not found", command.TenantId);
            return TenantErrors.NotFound(tenantId);
        }

        tenant.Suspend(clock);

        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}