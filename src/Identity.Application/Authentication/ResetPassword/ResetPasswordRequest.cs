using MediatR;

namespace Identity.Application.ResetPassword;

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword) : IRequest<ResetPasswordResponse>;
