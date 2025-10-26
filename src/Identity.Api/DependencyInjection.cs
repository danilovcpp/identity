using System.Reflection;
using Identity.Api.Core;

namespace Identity.Api;

public static class DependencyInjection
{
    /// <summary>
    /// Adds request handlers with optional pipeline configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurePipeline">Optional action to configure the pipeline behaviours.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRequestHandlers(
        this IServiceCollection services,
        Action<PipelineConfiguration>? configurePipeline = null)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Create and configure the pipeline
        var pipelineConfig = new PipelineConfiguration();
        configurePipeline?.Invoke(pipelineConfig);

        // Find all concrete request handlers (not behaviours)
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .Where(t => !t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IPipelineBehaviour<,>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            // Register the concrete handler
            services.AddScoped(handlerType);

            // Find the IRequestHandler<TRequest, TResponse> interface
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            var requestType = handlerInterface.GetGenericArguments()[0];
            var responseType = handlerInterface.GetGenericArguments()[1];

            // Build the pipeline of decorators
            services.AddScoped(handlerInterface, sp =>
            {
                // Start with the concrete handler
                IRequestHandler<object, object> current = (IRequestHandler<object, object>)sp.GetRequiredService(handlerType);

                // Build the pipeline from inside out (reverse order)
                // Behaviours are added in execution order, but we need to wrap from inside out
                var behaviourTypes = pipelineConfig.BehaviourTypes.Reverse().ToList();

                foreach (var behaviourType in behaviourTypes)
                {
                    var concreteBehaviourType = behaviourType.MakeGenericType(requestType, responseType);
                    current = (IRequestHandler<object, object>)CreateBehaviourInstance(sp, concreteBehaviourType, current);
                }

                return current;
            });
        }

        return services;
    }

    private static object CreateBehaviourInstance(
        IServiceProvider sp,
        Type behaviourType,
        object innerHandler)
    {
        // Get constructor parameters
        var constructor = behaviourType.GetConstructors().First();
        var parameters = constructor.GetParameters();
        var args = new List<object>();

        foreach (var param in parameters)
        {
            if (param.ParameterType.IsGenericType &&
                param.ParameterType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            {
                // This is the inner handler parameter
                args.Add(innerHandler);
            }
            else
            {
                // Resolve from DI container (e.g., ILogger)
                args.Add(sp.GetRequiredService(param.ParameterType));
            }
        }

        return Activator.CreateInstance(behaviourType, args.ToArray())!;
    }
}