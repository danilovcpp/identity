using Identity.Application.Common;
using Identity.Application.Tenants.Queries.GetTenantById;
using Identity.Application.Tenants.Queries.ListTenants;
using Identity.Domain.Tenants;

namespace Identity.Application.Tenants.Queries;

/// <summary>
/// Read-side projection port. Implemented in Infrastructure with EF IQueryable
/// projections directly into DTOs. Skips aggregate materialization for performance.
///
/// Note: query interfaces live in the slice they belong to (Tenants/Queries) rather
/// than under Abstractions/, because they're tightly coupled to the DTO shape and
/// are not really "ports" in the dependency-inversion sense — they're query helpers.
/// </summary>
public interface ITenantQueries
{
    Task<TenantDto?> GetByIdAsync(TenantId id, CancellationToken ct);

    Task<PagedResult<TenantListItemDto>> ListAsync(ListTenantsQuery filter, CancellationToken ct);
}