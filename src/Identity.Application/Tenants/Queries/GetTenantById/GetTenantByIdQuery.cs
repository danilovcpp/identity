using Identity.Application.Common;

namespace Identity.Application.Tenants.Queries.GetTenantById;

public sealed record GetTenantByIdQuery(Guid TenantId) : IQuery<TenantDto>;