using Identity.Application.Abstractions;
using Identity.Application.Avatar.Exceptions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Avatar;

public sealed class UploadAvatarRequestHandler : IRequestHandler<UploadAvatarRequest, UploadAvatarResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorageService _fileStorageService;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    ];

    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public UploadAvatarRequestHandler(
        UserManager<ApplicationUser> userManager,
        IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _fileStorageService = fileStorageService;
    }

    public async Task<UploadAvatarResponse> Handle(
        UploadAvatarRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        ValidateFile(request.FileStream, request.ContentType);

        // Удаляем старый аватар, если существует
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var oldFileName = Path.GetFileName(user.AvatarUrl);
            await _fileStorageService.DeleteFileAsync(oldFileName, cancellationToken);
        }

        // Генерируем уникальное имя файла
        var fileExtension = Path.GetExtension(request.FileName);
        var uniqueFileName = $"avatars/{user.Id}_{Guid.NewGuid()}{fileExtension}";

        // Загружаем новый аватар
        await _fileStorageService.UploadFileAsync(
            request.FileStream,
            uniqueFileName,
            request.ContentType,
            cancellationToken);

        // Получаем URL и обновляем пользователя
        var avatarUrl = _fileStorageService.GetFileUrl(uniqueFileName);
        user.AvatarUrl = avatarUrl;
        await _userManager.UpdateAsync(user);

        return new UploadAvatarResponse(avatarUrl);
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
