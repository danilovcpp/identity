using Identity.Core;

namespace Identity.Domain.Tenants;

public static class TenantErrors
{
    public static DomainError SlugAlreadyTaken(string slug) =>
        new("tenant.slug.already_taken",
            $"Tenant slug '{slug}' is already in use.",
            DomainErrorType.Conflict);

    public static DomainError NotFound(TenantId id) =>
        new("tenant.not_found",
            $"Tenant with id {id} not found.",
            DomainErrorType.NotFound);
}

public static class Validation
{
    public static DomainError Invalid(string message) =>
        new("validation.invalid", message);
}
