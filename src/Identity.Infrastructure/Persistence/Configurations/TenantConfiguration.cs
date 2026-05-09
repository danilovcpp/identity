using Identity.Core;
using Identity.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        //builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        // Slug is a value object; we store its string value in a citext column for
        // case-insensitive uniqueness without LOWER() everywhere.
        builder.Property(t => t.Slug)
            .HasConversion(s => s.Value, v => Slug.Create(v).Value)
            .HasColumnName("slug")
            .HasColumnType("citext")
            .IsRequired();

        builder.HasIndex(t => t.Slug)
            .IsUnique()
            .HasDatabaseName("uq_tenants_slug");

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<string>()        // store as text not int — readable, robust to enum changes
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.SuspendedAt).HasColumnName("suspended_at");
        builder.Property(t => t.DeletedAt).HasColumnName("deleted_at");

        // Settings is a value object containing nested value objects. We persist as
        // a single jsonb column with a value comparer for change tracking.
        // builder.OwnsOne(t => t.Settings, s =>
        // {
        //     s.ToJson("settings");
        //
        //     s.OwnsOne(x => x.PasswordPolicy);
        //     s.OwnsOne(x => x.SessionPolicy);
        //     s.OwnsOne(x => x.LockoutPolicy);
        // });

        // Use xmin as a concurrency token for optimistic concurrency.
        //builder.UseXminAsConcurrencyToken();

        // Domain events are not persisted — they're consumed by the SaveChanges interceptor.
        builder.Ignore(t => t.DomainEvents);

        // Index supporting list queries.
        builder.HasIndex(t => t.Status)
            .HasDatabaseName("ix_tenants_status")
            .HasFilter("deleted_at IS NULL");
    }
}