using Identity.Application.Common;

namespace Identity.Application.Accounts.Commands.UploadAvatar;

public sealed record UploadAvatarCommand(
    string UserId,
    Stream FileStream,
    string FileName,
    string ContentType) : ICommand<UploadAvatarResponse>;
