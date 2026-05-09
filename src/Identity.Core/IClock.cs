namespace Identity.Core;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
