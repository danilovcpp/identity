namespace Identity.Application.Authentication.Commands.Refresh.Exceptions;

public class InvalidRefreshTokenException() : Exception("Недействительный или истекший refresh token");