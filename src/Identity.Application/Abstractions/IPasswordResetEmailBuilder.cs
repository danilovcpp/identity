namespace Identity.Application.Abstractions;

public interface IPasswordResetEmailBuilder
{
    string CreateEmailBody(string passwordResetLink);
}
