using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(u => u.Tenant);
        
        builder.Property(u => u.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();
 
        builder.Ignore(u => u.DomainEvents);
    }
}