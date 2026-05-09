using System.Reflection;
using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Tenants;
using Identity.Domain.Users;
using Identity.Infrastructure.Persistence.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // After configurations register entities, sweep for strongly-typed ids.
        StronglyTypedIdConvention.ApplyTo(builder);
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        // Reasonable defaults for string columns.
        b.Properties<string>().HaveMaxLength(512);
    }
}