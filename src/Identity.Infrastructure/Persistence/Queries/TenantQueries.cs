using Identity.Application.Common;
using Identity.Application.Tenants.Queries;
using Identity.Application.Tenants.Queries.GetTenantById;
using Identity.Application.Tenants.Queries.ListTenants;
using Identity.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Queries;

internal sealed class TenantQueries(
    ApplicationDbContext context) : ITenantQueries
{
    public Task<TenantDto?> GetByIdAsync(TenantId id, CancellationToken ct)
        => context.Tenants
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TenantDto(
                t.Id.Value,
                t.Slug.Value,
                t.Name,
                t.Status.ToString(),
                t.CreatedAt,
                t.SuspendedAt))
            .FirstOrDefaultAsync(ct);

    public async Task<PagedResult<TenantListItemDto>> ListAsync(ListTenantsQuery q, CancellationToken ct)
    {
        var query = context.Tenants
            .AsNoTracking()
            .Where(t => t.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(q.SearchTerm))
        {
            var term = q.SearchTerm.Trim().ToLowerInvariant();
            // Postgres-specific: ILIKE against citext works without explicit lowering.
            query = query.Where(t =>
                EF.Functions.ILike(t.Name, $"%{term}%") ||
                EF.Functions.ILike(t.Slug.Value, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(q.StatusFilter)
            && Enum.TryParse<TenantStatus>(q.StatusFilter, ignoreCase: true, out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        // Cursor pagination: cursor encodes the last-seen Id. Because Guid v7 is
        // time-ordered, ordering by Id ascending = chronological. Decode and skip past.
        if (TryDecodeCursor(q.Cursor, out var afterId))
        {
            query = query.Where(t => t.Id.Value.CompareTo(afterId) > 0);
        }

        // Fetch one extra to detect whether there's another page.
        var page = await query
            .OrderBy(t => t.Id)
            .Take(q.PageSize + 1)
            .Select(t => new TenantListItemDto(
                t.Id.Value, t.Slug.Value, t.Name, t.Status.ToString(), t.CreatedAt))
            .ToListAsync(ct);

        var hasMore = page.Count > q.PageSize;
        if (hasMore) page.RemoveAt(page.Count - 1);

        var nextCursor = hasMore ? EncodeCursor(page[^1].Id) : null;
        return new PagedResult<TenantListItemDto>(page, nextCursor, hasMore);
    }

    private static bool TryDecodeCursor(string? cursor, out Guid id)
    {
        id = Guid.Empty;
        if (string.IsNullOrWhiteSpace(cursor)) return false;
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            id = new Guid(bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string EncodeCursor(Guid id)
        => Convert.ToBase64String(id.ToByteArray());
}