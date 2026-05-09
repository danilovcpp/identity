using MediatR;

namespace Identity.Application.ForgotPassword;

public record ForgotPasswordRequest(string Email) : IRequest<ForgotPasswordResponse>;
