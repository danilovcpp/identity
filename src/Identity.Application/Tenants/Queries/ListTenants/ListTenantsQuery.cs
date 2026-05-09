using Identity.Application.Common;

namespace Identity.Application.Tenants.Queries.ListTenants;

public sealed record ListTenantsQuery(
    string? SearchTerm,
    string? StatusFilter,
    string? Cursor,
    int PageSize = 50) : IQuery<PagedResult<TenantListItemDto>>;
