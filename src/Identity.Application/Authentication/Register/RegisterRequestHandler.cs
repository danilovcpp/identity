using Identity.Application.Abstractions;
using Identity.Application.Register;
using Identity.Application.Register.Exceptions;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Authentication.Register;

public sealed class RegisterRequestHandler(
    UserManager<ApplicationUser> userManager,
    IEmailConfirmationService emailConfirmationService) : IRequestHandler<RegisterRequest, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
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