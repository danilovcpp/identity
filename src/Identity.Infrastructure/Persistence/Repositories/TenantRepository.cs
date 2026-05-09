using Identity.Application.Abstractions.Persistence;
using Identity.Core;
using Identity.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class TenantRepository(
    ApplicationDbContext context) : ITenantRepository
{
    public Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct)
        => context.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<Tenant?> GetBySlugAsync(Slug slug, CancellationToken ct)
        => context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug, ct);

    public Task<bool> SlugExistsAsync(Slug slug, CancellationToken ct)
        => context.Tenants.AnyAsync(t => t.Slug == slug, ct);

    public void Add(Tenant tenant)
        => context.Tenants.Add(tenant);

    public void Remove(Tenant tenant)
        => context.Tenants.Remove(tenant);
}