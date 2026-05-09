using System.Linq.Expressions;
using Identity.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Infrastructure.Persistence.Conventions;

internal static class StronglyTypedIdConvention
{
    /// <summary>
    /// For every property on every entity whose CLR type implements IStronglyTypedId,
    /// register a value converter that maps it to/from Guid. Saves us writing ~50
    /// converters by hand.
    /// </summary>
    public static void ApplyTo(ModelBuilder model)
    {
        foreach (var entity in model.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                var clr = property.ClrType;
                var underlying = Nullable.GetUnderlyingType(clr) ?? clr;
                if (!typeof(IStronglyTypedId).IsAssignableFrom(underlying))
                    continue;

                var converter = BuildConverter(underlying);
                property.SetValueConverter(converter);
            }
        }
    }

    private static ValueConverter BuildConverter(Type idType)
    {
        // Build:  id => id.Value
        var idParam = Expression.Parameter(idType, "id");
        var toGuid = Expression.Lambda(
            Expression.Property(idParam, nameof(IStronglyTypedId.Value)),
            idParam);

        // Build:  guid => new TId(guid)  (record structs have a constructor taking Guid)
        var ctor = idType.GetConstructor([typeof(Guid)])
                   ?? throw new InvalidOperationException(
                       $"Strongly-typed id {idType.Name} must have a public constructor taking a Guid.");
        var guidParam = Expression.Parameter(typeof(Guid), "guid");
        var fromGuid = Expression.Lambda(
            Expression.New(ctor, guidParam),
            guidParam);

        var converterType = typeof(ValueConverter<,>).MakeGenericType(idType, typeof(Guid));
        return (ValueConverter)Activator.CreateInstance(converterType, toGuid, fromGuid, null)!;
    }
}