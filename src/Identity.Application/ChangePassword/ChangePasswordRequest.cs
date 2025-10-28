using MediatR;

namespace Identity.Application.ChangePassword;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : IRequest<ChangePasswordResponse>;
