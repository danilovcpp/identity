using Identity.Api.Controllers.Register;
using Identity.Api.Core;

namespace Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddRequestHandlers(this IServiceCollection services)
    {
        var assembly = typeof(RegisterRequestHandler).Assembly;
        var handlerInterface = typeof(IRequestHandler<,>);
        var handlerImplementationTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface));

        foreach (var handlerImplementationType in handlerImplementationTypes)
        {
            services.AddScoped(handlerImplementationType);
        }

        return services;
    }
}