using Identity.Application.Common;

namespace Identity.Application.Accounts.Commands.DeleteAvatar;

public sealed record DeleteAvatarCommand(string UserId) : ICommand;
