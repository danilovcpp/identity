using Identity.Application.Abstractions.Integrations;
using Identity.Application.Accounts.Commands.UploadAvatar.Exceptions;
using Identity.Application.Common;
using Identity.Core;

namespace Identity.Application.Accounts.Commands.UploadAvatar;

public sealed class UploadAvatarCommandHandler(
    IFileStorageService fileStorageService) : ICommandHandler<UploadAvatarCommand, UploadAvatarResponse>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    ];

    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public async Task<Result<UploadAvatarResponse>> HandleAsync(
        UploadAvatarCommand command,
        CancellationToken cancellationToken)
    {
        // var user = await _userManager.FindByIdAsync(command.UserId);
        // if (user is null)
        // {
        //     throw new KeyNotFoundException("User not found.");
        // }
        //
        // ValidateFile(command.FileStream, command.ContentType);
        //
        // // Удаляем старый аватар, если существует
        // if (!string.IsNullOrEmpty(user.AvatarUrl))
        // {
        //     var oldFileName = Path.GetFileName(user.AvatarUrl);
        //     await fileStorageService.DeleteFileAsync(oldFileName, cancellationToken);
        // }
        //
        // // Генерируем уникальное имя файла
        // var fileExtension = Path.GetExtension(command.FileName);
        // var uniqueFileName = $"avatars/{user.Id}_{Guid.NewGuid()}{fileExtension}";
        //
        // // Загружаем новый аватар
        // await fileStorageService.UploadFileAsync(
        //     command.FileStream,
        //     uniqueFileName,
        //     command.ContentType,
        //     cancellationToken);
        //
        // // Получаем URL и обновляем пользователя
        // var avatarUrl = fileStorageService.GetFileUrl(uniqueFileName);
        // user.AvatarUrl = avatarUrl;
        // await _userManager.UpdateAsync(user);

        return new UploadAvatarResponse("avatarUrl");
    }

    private static void ValidateFile(Stream fileStream, string contentType)
    {
        if (!AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
        {
            throw new InvalidFileTypeException();
        }

        if (fileStream.Length > MaxFileSizeInBytes)
        {
            throw new FileTooLargeException(MaxFileSizeInBytes / 1024 / 1024);
        }
    }
}
