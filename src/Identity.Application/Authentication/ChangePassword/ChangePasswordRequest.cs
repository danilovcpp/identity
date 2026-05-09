using Identity.Application.ChangePassword;
using MediatR;

namespace Identity.Application.Authentication.ChangePassword;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : IRequest<ChangePasswordResponse>;
