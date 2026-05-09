using Identity.Domain.Users;

namespace Identity.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct);
    void Add(User user);
}