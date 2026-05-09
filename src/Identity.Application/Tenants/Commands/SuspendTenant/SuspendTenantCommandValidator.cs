using FluentValidation;

namespace Identity.Application.Tenants.Commands.SuspendTenant;

public sealed class SuspendTenantCommandValidator : AbstractValidator<SuspendTenantCommand>
{
    public SuspendTenantCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty();
    }
}