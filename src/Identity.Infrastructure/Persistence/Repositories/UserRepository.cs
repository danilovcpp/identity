using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class UserRepository(
    ApplicationDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(UserId id, CancellationToken ct)
        => context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public void Add(User user)
        => context.Users.Add(user);
}