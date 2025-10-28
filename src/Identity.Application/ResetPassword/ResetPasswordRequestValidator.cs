using FluentValidation;

namespace Identity.Application.ResetPassword;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат email");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Токен обязателен");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");
    }
}
