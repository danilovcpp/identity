using Identity.Api.Abstractions;
using Identity.Api.Controllers.Register.Exceptions;
using Identity.Api.Core;
using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Controllers.Register;

public class RegisterRequestHandler(
    UserManager<ApplicationUser> userManager,
    IEmailConfirmationService emailConfirmationService) : IRequestHandler<RegisterRequest, RegisterResponse>
{
    public async Task<RegisterResponse> HandleAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };
        
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new RegisterException();
        }

        await emailConfirmationService.SendConfirmationLink(user);

        return new RegisterResponse();
    }
}