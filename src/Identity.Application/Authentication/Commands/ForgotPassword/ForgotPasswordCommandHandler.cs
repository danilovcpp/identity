using Identity.Application.Abstractions;
using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IPasswordResetEmailService passwordResetEmailService) : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    public async Task<Result<ForgotPasswordResponse>> HandleAsync(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        // var user = await userManager.FindByEmailAsync(command.Email);
        //
        // // По соображениям безопасности не раскрываем, существует ли пользователь
        // // Возвращаем успех в любом случае, чтобы избежать enumeration атак
        // if (user is null)
        // {
        //     return new ForgotPasswordResponse();
        // }
        //
        // // Проверяем, подтвержден ли email (опционально, можно убрать)
        // if (!await userManager.IsEmailConfirmedAsync(user))
        // {
        //     return new ForgotPasswordResponse();
        // }
        //
        // await passwordResetEmailService.SendPasswordResetLink(user);

        return new ForgotPasswordResponse();
    }
}
