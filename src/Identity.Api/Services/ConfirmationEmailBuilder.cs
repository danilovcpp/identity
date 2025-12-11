using Identity.Application.Abstractions;

namespace Identity.Api.Services;

public class ConfirmationEmailBuilder : IConfirmationEmailBuilder
{
    public string CreateEmailBody(string confirmationLink)
        => $@"<h2>Добро пожаловать!</h2>
               <p>Пожалуйста, подтвердите ваш email, перейдя по ссылке:</p>
               <p><a href='{confirmationLink}'>Подтвердить email</a></p>
               <p>Или скопируйте ссылку в браузер:</p>
               <p>{confirmationLink}</p>";
}