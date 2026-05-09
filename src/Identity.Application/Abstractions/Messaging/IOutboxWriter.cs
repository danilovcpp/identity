namespace Identity.Application.Abstractions.Messaging;

public interface IOutboxWriter
{
    /// <summary>
    /// Enqueue a message in the outbox. The message is persisted in the same
    /// transaction as the surrounding business change. The dispatcher worker
    /// picks it up afterward.
    /// </summary>
    void Enqueue(string messageType, object payload, Guid? correlationId = null);
}