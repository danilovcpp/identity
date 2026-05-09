namespace Identity.Application.Authentication.Commands.Register;

public sealed class RegisterResponse
{
    public string Message { get; set; } = "Регистрация успешна. Проверьте email для подтверждения.";
}