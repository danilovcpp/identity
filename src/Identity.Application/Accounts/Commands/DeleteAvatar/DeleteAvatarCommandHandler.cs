using Identity.Application.Abstractions.Integrations;
using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Accounts.Commands.DeleteAvatar;

public sealed class DeleteAvatarCommandHandler(
    IFileStorageService fileStorageService) : ICommandHandler<DeleteAvatarCommand>
{
    public async Task<Result<Unit>> HandleAsync(
        DeleteAvatarCommand command,
        CancellationToken cancellationToken)
    {
        // var user = await _userManager.FindByIdAsync(command.UserId);
        // if (user is null)
        // {
        //     throw new KeyNotFoundException("User not found.");
        // }
        //
        // if (string.IsNullOrEmpty(user.AvatarUrl))
        // {
        //     return; // Нет аватара для удаления
        // }
        //
        // var fileName = Path.GetFileName(user.AvatarUrl);
        // await fileStorageService.DeleteFileAsync(fileName, cancellationToken);
        //
        // user.AvatarUrl = null;
        // await _userManager.UpdateAsync(user);
        throw new NotImplementedException();
    }
}
