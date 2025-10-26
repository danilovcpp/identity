namespace Identity.Api.Controllers.Revoke.Exceptions;

public class RefreshTokenAlreadyRevokedException() : Exception("Refresh token уже отозван");