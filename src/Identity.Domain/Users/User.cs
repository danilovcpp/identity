using Identity.Core;
using Identity.Domain.Users.Events;

namespace Identity.Domain.Users;

public class User : AggregateRoot<UserId>, IHasDomainEvents
{
    private User(
        UserId id,
        string accountName,
        string firstName,
        string lastName,
        DateTimeOffset createdAt) : base(id)
    {
        AccountName = accountName;
        FirstName = firstName;
        LastName = lastName;
    }

    public string AccountName { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public static Result<User> Create(
        string accountName,
        string firstName,
        string lastName,
        IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        // todo: check if null

        var user = new User(
            id: UserId.New(),
            accountName: accountName,
            firstName: firstName,
            lastName: lastName,
            createdAt: clock.UtcNow);
        
        user.RaiseDomainEvent(new UserCreated(user.Id, user.CreatedAt));
        return user;
    }

    public Result Activate(IClock clock)
    {
        return Result.Failure(new DomainError("notimplemented", "Notimplemented"));
    }

    public Result Block(IClock clock)
    {
        return Result.Failure(new DomainError("notimplemented", "Notimplemented"));
    }

    public Result Unblock(DateTimeOffset now)
    {
        return Result.Failure(new DomainError("notimplemented", "Notimplemented"));
    }
}