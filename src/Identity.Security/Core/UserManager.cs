using Identity.Security.Abstractions;

namespace Identity.Security.Core;

public class UserManager<TUser> where TUser : class, IUser
{
    private readonly IUserStore<TUser> _store;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILookupNormalizer _normalizer;
    private readonly IEnumerable<IUserValidator<TUser>> _userValidators;
    private readonly IEnumerable<IPasswordValidator<TUser>> _passwordValidators;

    public UserManager(
        IUserStore<TUser> store,
        IPasswordHasher passwordHasher,
        ILookupNormalizer normalizer,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators)
    {
        _store = store;
        _passwordHasher = passwordHasher;
        _normalizer = normalizer;
        _userValidators = userValidators;
        _passwordValidators = passwordValidators;
    }

    public async Task<IdentityResult> CreateAsync(TUser user, string password)
    {
        user.NormalizedUserName = _normalizer.Normalize(user.UserName);

        foreach (var validator in _userValidators)
        {
            var r = await validator.ValidateAsync(user);
            if (!r.Succeeded) return r;
        }

        foreach (var validator in _passwordValidators)
        {
            var r = await validator.ValidateAsync(user, password);
            if (!r.Succeeded) return r;
        }

        user.PasswordHash = _passwordHasher.HashPassword(password);

        await _store.CreateAsync(user);
        return IdentityResult.Success();
    }

    public async Task<TUser?> FindByNameAsync(string userName)
    {
        var norm = _normalizer.Normalize(userName);
        return await _store.FindByNameAsync(norm);
    }

    public bool CheckPassword(TUser user, string password) =>
        _passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
}