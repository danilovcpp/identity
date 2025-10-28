using Identity.Api.Abstractions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.ForgotPassword;

public class ForgotPasswordRequestHandler(
    UserManager<ApplicationUser> userManager,
    IPasswordResetEmailService passwordResetEmailService) : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        // По соображениям безопасности не раскрываем, существует ли пользователь
        // Возвращаем успех в любом случае, чтобы избежать enumeration атак
        if (user is null)
        {
            return new ForgotPasswordResponse();
        }

        // Проверяем, подтвержден ли email (опционально, можно убрать)
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            return new ForgotPasswordResponse();
        }

        await passwordResetEmailService.SendPasswordResetLink(user);

        return new ForgotPasswordResponse();
    }
}
