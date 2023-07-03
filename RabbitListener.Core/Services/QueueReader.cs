namespace RabbitListener.Core.Services;

public class QueueReader
{
    public static readonly Queue<string?> MessageQueue = new Queue<string?>();

    public async Task<string?> GetMessageFromQueue()
    {
        await Task.CompletedTask;
        return MessageQueue.Count == 0 ? null : MessageQueue.Dequeue();
    }
}