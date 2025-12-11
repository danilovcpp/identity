using Identity.Application.Abstractions;

namespace Identity.Api.Services;

public class PasswordResetEmailBuilder : IPasswordResetEmailBuilder
{
    public string CreateEmailBody(string passwordResetLink)
        => $@"<h2>Сброс пароля</h2>
               <p>Вы запросили сброс пароля. Перейдите по ссылке ниже, чтобы установить новый пароль:</p>
               <p><a href='{passwordResetLink}'>Сбросить пароль</a></p>
               <p>Или скопируйте ссылку в браузер:</p>
               <p>{passwordResetLink}</p>
               <p>Если вы не запрашивали сброс пароля, проигнорируйте это письмо.</p>
               <p>Ссылка действительна в течение ограниченного времени.</p>";
}
