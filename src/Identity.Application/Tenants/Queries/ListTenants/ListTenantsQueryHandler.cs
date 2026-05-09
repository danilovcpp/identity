using Identity.Application.Common;
using Identity.Core;
using Identity.Domain.Tenants;

namespace Identity.Application.Tenants.Queries.ListTenants;

public sealed class ListTenantsQueryHandler(ITenantQueries queries)
    : IQueryHandler<ListTenantsQuery, PagedResult<TenantListItemDto>>
{
    private const int MaxPageSize = 200;

    public async Task<Result<PagedResult<TenantListItemDto>>> HandleAsync(ListTenantsQuery q, CancellationToken ct)
    {
        if (q.PageSize is < 1 or > MaxPageSize)
            return Validation.Invalid(
                $"Page size must be between 1 and {MaxPageSize}.");

        return await queries.ListAsync(q, ct);
    }
}