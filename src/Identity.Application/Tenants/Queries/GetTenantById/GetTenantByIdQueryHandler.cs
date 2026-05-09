using Identity.Application.Common;
using Identity.Core;
using Identity.Domain.Tenants;

namespace Identity.Application.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdQueryHandler(ITenantQueries queries)
    : IQueryHandler<GetTenantByIdQuery, TenantDto>
{
    public async Task<Result<TenantDto>> HandleAsync(GetTenantByIdQuery query, CancellationToken ct)
    {
        var tenantId = new TenantId(query.TenantId);
        var dto = await queries.GetByIdAsync(tenantId, ct);
        if (dto is null)
            return TenantErrors.NotFound(tenantId);

        return dto;
    }
}