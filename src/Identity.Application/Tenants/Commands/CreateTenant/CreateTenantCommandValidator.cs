using FluentValidation;

namespace Identity.Application.Tenants.Commands.CreateTenant;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty()
            .Length(3, 63);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}