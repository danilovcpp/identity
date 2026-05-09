namespace Identity.Application.Abstractions.Persistence;

public sealed class UniqueConstraintViolationException(string constraintName, Exception inner)
    : Exception($"Unique constraint '{constraintName}' violated.", inner)
{
    public string ConstraintName { get; } = constraintName;
}