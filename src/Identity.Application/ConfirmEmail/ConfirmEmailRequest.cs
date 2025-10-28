using MediatR;

namespace Identity.Application.ConfirmEmail;

public record ConfirmEmailRequest(string UserId, string Token) : IRequest<ConfirmEmailResponse>;