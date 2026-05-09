using MediatR;

namespace Identity.Application.Avatar;

public sealed record UploadAvatarRequest(
    string UserId,
    Stream FileStream,
    string FileName,
    string ContentType) : IRequest<UploadAvatarResponse>;
