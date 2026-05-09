using FluentValidation;

namespace Identity.Application.Authentication.ChangePassword;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Текущий пароль обязателен");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Новый пароль обязателен")
            .MinimumLength(6)
            .WithMessage("Новый пароль должен содержать минимум 6 символов")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("Новый пароль должен отличаться от текущего");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Подтверждение пароля обязательно")
            .Equal(x => x.NewPassword)
            .WithMessage("Пароли не совпадают");
    }
}
