using Identity.Application.Abstractions.Integrations;
using Identity.Application.Abstractions.Messaging;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common;
using Identity.Application.Tenants.Queries;
using Identity.Core;
using Identity.Infrastructure.Integrations.Email;
using Identity.Infrastructure.Messaging;
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
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDomainEventDispatcher, LoggingDomainEventDispatcher>();
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetConnectionString("IdentityDatabase"))
                .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>()));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IClock, Clock>();
        services.AddScoped<IOutboxWriter, OutboxWriter>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories (write side).
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        // ...IUserRepository, IRefreshTokenRepository, etc.

        // Query services (read side).
        services.AddScoped<ITenantQueries, TenantQueries>();
        // ...IUserQueries, etc.

        // Decorate every command handler with the validation behavior.
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
        // For logging or transaction-scope decorators, add more Decorate calls in order.

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
}