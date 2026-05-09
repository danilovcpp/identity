using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Infrastructure.Persistence.Conventions;

public sealed class StronglyTypedIdValueConverter<TId>
    : ValueConverter<TId, Guid>
    where TId : struct
{
    public StronglyTypedIdValueConverter(Func<Guid, TId> factory, Func<TId, Guid> selector)
        : base(id => selector(id), guid => factory(guid))
    {
    }
}