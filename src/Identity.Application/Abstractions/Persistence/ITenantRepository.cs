using Identity.Core;
using Identity.Domain.Tenants;

namespace Identity.Application.Abstractions.Persistence;

/// <summary>
/// Write-side repository for the Tenant aggregate. Methods return aggregates
/// or check existence. Read-only list/search/projection queries do NOT live here —
/// they live as IQueryable projections in Infrastructure/Queries/Tenants and are
/// consumed by query handlers.
///
/// This separation matters: write-side repos load full aggregates with all their
/// children for invariant-preserving mutations; query projections load only the
/// columns the API actually returns. Conflating them produces over-fetching on
/// reads or under-fetching on writes.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Load the full Tenant aggregate by id, including any owned children.
    /// </summary>
    Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct);

    /// <summary>
    /// Load the full aggregate by its slug. Used for slug-based lookups during auth.
    /// </summary>
    Task<Tenant?> GetBySlugAsync(Slug slug, CancellationToken ct);

    /// <summary>
    /// Existence check, used by command handlers for uniqueness validation.
    /// </summary>
    Task<bool> SlugExistsAsync(Slug slug, CancellationToken ct);

    /// <summary>
    /// Track a new tenant for insertion at SaveChanges.
    /// </summary>
    void Add(Tenant tenant);

    /// <summary>
    /// Track removal. Hard delete is rare (we use soft-delete via Tenant.MarkDeleted);
    /// this is here for tests and admin tooling.
    /// </summary>
    void Remove(Tenant tenant);
}