namespace Identity.Application.Abstractions;

public interface IConfirmationEmailBuilder
{
    string CreateEmailBody(string confirmationLink);
}