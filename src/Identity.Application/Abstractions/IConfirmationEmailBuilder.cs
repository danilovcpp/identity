namespace Identity.Api.Abstractions;

public interface IConfirmationEmailBuilder
{
    string CreateEmailBody(string confirmationLink);
}