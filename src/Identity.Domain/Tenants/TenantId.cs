using Identity.Core;

namespace Identity.Domain.Tenants;

public readonly record struct TenantId(Guid Value) : IStronglyTypedId
{
    public static TenantId New()
        => new(Guid.CreateVersion7());

    public override string ToString()
        => Value.ToString();
}