namespace Identity.Application.Authentication.Commands.ResetPassword.Exceptions;

public class InvalidPasswordResetTokenException : Exception
{
    public InvalidPasswordResetTokenException()
        : base("Недействительный или просроченный токен сброса пароля")
    {
    }
}
