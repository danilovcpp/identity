namespace Identity.Application.ResetPassword.Exceptions;

public class InvalidPasswordResetTokenException : Exception
{
    public InvalidPasswordResetTokenException()
        : base("Недействительный или просроченный токен сброса пароля")
    {
    }
}
