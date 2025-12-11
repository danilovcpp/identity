namespace Identity.Security.Abstractions;

public interface IUserStore<TUser> where TUser : class, IUser
{
    Task<TUser?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken ct = default);
    Task CreateAsync(TUser user, CancellationToken ct = default);
    Task UpdateAsync(TUser user, CancellationToken ct = default);
}