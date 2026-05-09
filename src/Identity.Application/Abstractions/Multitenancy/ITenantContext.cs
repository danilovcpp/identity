using Identity.Domain.Tenants;

namespace Identity.Application.Abstractions.Multitenancy;

/// <summary>
/// Resolves the tenant for the current request. Implemented in the API layer
/// (typically resolved from subdomain, header, or claim). Application code
/// uses this to filter writes and reads to the current tenant.
///
/// Some operations (admin global, system) need to bypass the filter. The
/// implementation should support an explicit "ambient" override scope.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Null when the call is genuinely tenant-less (e.g. global admin).
    /// </summary>
    TenantId? CurrentTenantId { get; }

    bool IsAmbient { get; } // explicit "ignore tenant filter" scope is active
}