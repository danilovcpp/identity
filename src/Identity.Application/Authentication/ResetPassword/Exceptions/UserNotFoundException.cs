namespace Identity.Application.ResetPassword.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException()
        : base("Пользователь не найден")
    {
    }
}
