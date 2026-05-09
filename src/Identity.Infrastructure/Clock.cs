using Identity.Core;

namespace Identity.Infrastructure;

public class Clock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}