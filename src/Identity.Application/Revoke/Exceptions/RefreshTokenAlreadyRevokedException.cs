namespace Identity.Application.Revoke.Exceptions;

public class RefreshTokenAlreadyRevokedException() : Exception("Refresh token уже отозван");