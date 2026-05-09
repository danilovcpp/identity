using Identity.Application.Common;

namespace Identity.Application.Tenants.Commands.CreateTenant;

public sealed record CreateTenantCommand(string Slug, string Name) : ICommand<Guid>;