namespace Identity.Api.Controllers.Refresh.Exceptions;

public class InvalidRefreshTokenException() : Exception("Недействительный или истекший refresh token");