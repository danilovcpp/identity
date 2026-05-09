using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Authentication.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    ) : ICommandHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    public async Task<Result<ConfirmEmailResponse>> HandleAsync(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        // if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        //     return BadRequest(new { message = "UserId и token обязательны" });

        // var user = await userManager.FindByIdAsync(command.UserId) ?? throw new Exception("User not found");
        // var result = await userManager.ConfirmEmailAsync(user, command.Token);
        // if (!result.Succeeded)
        // {
        //     throw new Exception("Error while confirming email");
        // }

        return new ConfirmEmailResponse();
    }
}