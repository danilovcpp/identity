namespace Identity.Domain.Tenants;

public static class TenantErrorMessages
{
    public const string NameEmpty = "Tenant name is required.";
    public const string NameTooLong = "Tenant name must be at most 200 characters.";
    public const string Deleted = "Cannot suspend a deleted tenant.";
}