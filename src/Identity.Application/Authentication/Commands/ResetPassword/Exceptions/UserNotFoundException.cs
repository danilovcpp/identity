namespace Identity.Application.Authentication.Commands.ResetPassword.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException()
        : base("Пользователь не найден")
    {
    }
}
