namespace Identity.Api.Controllers.ConfirmEmail;

public record ConfirmEmailRequest(string UserId, string Token);