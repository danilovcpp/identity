using MediatR;

namespace Identity.Application.Avatar;

public sealed record DeleteAvatarRequest(string UserId) : IRequest;
