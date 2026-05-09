namespace Identity.Application.Refresh.Exceptions;

public class InvalidRefreshTokenException() : Exception("Недействительный или истекший refresh token");