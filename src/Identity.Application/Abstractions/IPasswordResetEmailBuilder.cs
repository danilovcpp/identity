namespace Identity.Api.Abstractions;

public interface IPasswordResetEmailBuilder
{
    string CreateEmailBody(string passwordResetLink);
}
