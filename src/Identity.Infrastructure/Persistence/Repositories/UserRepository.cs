using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Users;

namespace Identity.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    public Task<User?> GetByIdAsync(UserId id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public void Add(User user)
    {
        throw new NotImplementedException();
    }
}