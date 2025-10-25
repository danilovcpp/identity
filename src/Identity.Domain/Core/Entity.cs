namespace Identity.Domain.Core;

public abstract class Entity
{
    public int Id { get; protected set; }
}

public abstract class Entity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; protected set; } = default!;
}