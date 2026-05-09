using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Integrations;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common;
using Identity.Application.Tenants.Queries;
using Identity.Core;
using Identity.Infrastructure.Integrations.Email;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Interceptors;
using Identity.Infrastructure.Persistence.Queries;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Pipeline;
using Identity.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("IdentityDatabase")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        //     {
        //         options.Password.RequiredLength = 6;
        //         options.Password.RequireNonAlphanumeric = false;
        //         options.SignIn.RequireConfirmedEmail = true;
        //     })
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddDefaultTokenProviders();

        services.AddScoped<IEmailSender, EmailSender>();

        // MinIO configuration
        services.Configure<MinioSettings>(configuration.GetSection(MinioSettings.SectionName));

        services.AddSingleton<IMinioClient>(sp =>
        {
            var settings = configuration.GetSection(MinioSettings.SectionName).Get<MinioSettings>()
                ?? throw new InvalidOperationException("MinIO settings are not configured.");

            return new MinioClient()
                .WithEndpoint(settings.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        });

        services.AddScoped<IFileStorageService, MinioFileStorageService>();

        return services;
    }
    
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services, IConfiguration cfg)
    {
        //services.AddSingleton<IClock, SystemClock>();

        services.AddScoped<DomainEventDispatchInterceptor>();
        //services.AddScoped<OutboxInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options
                .UseNpgsql(cfg.GetConnectionString("IdentityDatabase"), npg =>
                {
                    // npg.MigrationsHistoryTable("__ef_migrations_history", IdentityDbContext.Schema);
                    // npg.EnableRetryOnFailure(3);
                })
                .AddInterceptors(
                    sp.GetRequiredService<DomainEventDispatchInterceptor>());
            //sp.GetRequiredService<OutboxInterceptor>());

            // Off in production; flip on for local debugging.
            // options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories (write side).
        services.AddScoped<ITenantRepository, TenantRepository>();
        // ...IUserRepository, IRefreshTokenRepository, etc.

        // Query services (read side).
        services.AddScoped<ITenantQueries, TenantQueries>();
        // ...IUserQueries, etc.

        // Decorate every command handler with the validation behavior.
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        // For logging or transaction-scope decorators, add more Decorate calls in order.

        return services;
    }
}