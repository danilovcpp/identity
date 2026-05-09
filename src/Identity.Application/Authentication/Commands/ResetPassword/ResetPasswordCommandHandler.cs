using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    ) : ICommandHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<Result<ResetPasswordResponse>> HandleAsync(
        ResetPasswordCommand command,
        CancellationToken ct)
    {
        return new ResetPasswordResponse();
    }
}
