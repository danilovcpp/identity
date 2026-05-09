namespace Identity.Application.Tenants.Queries.ListTenants;

public sealed record TenantListItemDto(
    Guid Id,
    string Slug,
    string Name,
    string Status,
    DateTimeOffset CreatedAt);