using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Avatar;

public sealed class DeleteAvatarRequestHandler : IRequestHandler<DeleteAvatarRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorageService _fileStorageService;

    public DeleteAvatarRequestHandler(
        UserManager<ApplicationUser> userManager,
        IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(DeleteAvatarRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (string.IsNullOrEmpty(user.AvatarUrl))
        {
            return; // Нет аватара для удаления
        }

        var fileName = Path.GetFileName(user.AvatarUrl);
        await _fileStorageService.DeleteFileAsync(fileName, cancellationToken);

        user.AvatarUrl = null;
        await _userManager.UpdateAsync(user);
    }
}
