namespace Identity.Application.Tenants.Queries.GetTenantById;

public sealed record TenantDto(
    Guid Id,
    string Slug,
    string Name,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SuspendedAt);