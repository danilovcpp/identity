using Identity.Application.Abstractions.Messaging;

namespace Identity.Infrastructure.Messaging;

public class OutboxWriter : IOutboxWriter
{
    public void Enqueue(string messageType, object payload, Guid? correlationId = null)
    {
        // empty
    }
}