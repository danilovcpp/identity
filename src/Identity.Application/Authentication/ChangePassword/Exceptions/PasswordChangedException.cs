namespace Identity.Application.ChangePassword.Exceptions;

public class PasswordChangeFailedException(IEnumerable<string> errors)
    : Exception($"Не удалось изменить пароль: {string.Join(", ", errors)}")
{
    public IEnumerable<string> Errors { get; } = errors;
}
