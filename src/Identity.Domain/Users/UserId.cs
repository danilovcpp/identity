using Identity.Core;

namespace Identity.Domain.Users;

public readonly record struct UserId(Guid Value) : IStronglyTypedId
{
    public static UserId New()
        => new(Guid.CreateVersion7());

    public override string ToString()
        => Value.ToString();
}