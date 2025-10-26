using Identity.Api.Core;

namespace Identity.Api.Controllers.ConfirmEmail;

public record ConfirmEmailRequest(string UserId, string Token) : IRequest<ConfirmEmailResponse>;