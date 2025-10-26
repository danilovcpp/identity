namespace Identity.Api.Core;

/// <summary>
/// Configuration for the request handler pipeline.
/// Behaviours are executed in the order they are added.
/// </summary>
public class PipelineConfiguration
{
    private readonly List<Type> _behaviourTypes = new();

    /// <summary>
    /// Gets the registered behaviour types in order of execution.
    /// </summary>
    internal IReadOnlyList<Type> BehaviourTypes => _behaviourTypes.AsReadOnly();

    /// <summary>
    /// Adds a pipeline behaviour to be applied to all request handlers.
    /// Behaviours are executed in the order they are added.
    /// </summary>
    /// <param name="behaviourType">The generic type definition of the behaviour (e.g., typeof(LoggingBehaviour&lt;,&gt;))</param>
    /// <returns>The configuration instance for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the type is not a valid pipeline behaviour.</exception>
    public PipelineConfiguration AddBehaviour(Type behaviourType)
    {
        if (!behaviourType.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"Behaviour type must be a generic type definition (e.g., typeof(LoggingBehaviour<,>)). " +
                $"Received: {behaviourType.Name}",
                nameof(behaviourType));
        }

        if (!IsValidBehaviourType(behaviourType))
        {
            throw new ArgumentException(
                $"Type {behaviourType.Name} must implement IPipelineBehaviour<TRequest, TResponse>.",
                nameof(behaviourType));
        }

        _behaviourTypes.Add(behaviourType);
        return this;
    }

    /// <summary>
    /// Adds a pipeline behaviour to be applied to all request handlers.
    /// Behaviours are executed in the order they are added.
    /// </summary>
    /// <typeparam name="TBehaviour">The behaviour type (must be an open generic type).</typeparam>
    /// <returns>The configuration instance for chaining.</returns>
    public PipelineConfiguration AddBehaviour<TBehaviour>()
        where TBehaviour : class
    {
        return AddBehaviour(typeof(TBehaviour));
    }

    private static bool IsValidBehaviourType(Type behaviourType)
    {
        // Check if the type has the correct generic parameters (TRequest, TResponse)
        if (behaviourType.GetGenericArguments().Length != 2)
        {
            return false;
        }

        // Try to construct the interface type to check if it implements IPipelineBehaviour<,>
        var genericArgs = behaviourType.GetGenericArguments();
        var pipelineInterface = typeof(IPipelineBehaviour<,>).MakeGenericType(genericArgs);

        return behaviourType.GetInterfaces()
            .Any(i => i.IsGenericType &&
                     i.GetGenericTypeDefinition() == typeof(IPipelineBehaviour<,>));
    }
}
