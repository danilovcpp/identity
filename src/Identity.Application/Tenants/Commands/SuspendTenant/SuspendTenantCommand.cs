using Identity.Application.Common;

namespace Identity.Application.Tenants.Commands.SuspendTenant;

public sealed record SuspendTenantCommand(Guid TenantId) : ICommand;