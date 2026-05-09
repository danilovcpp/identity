namespace Identity.Core;

public interface IStronglyTypedId
{
    Guid Value { get; }
}
