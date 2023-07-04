namespace RabbitListener.Core.Services;

public class QueueManager
{
    public static readonly Queue<string?> MessageQueue = new Queue<string?>();

    public static async Task<string?> ReadMessageFromQueue()
    {
        await Task.CompletedTask;
        return MessageQueue.Count == 0 ? null : MessageQueue.Dequeue();
    }
    
    public static async Task FetchMessage(string message)
    {
        await Task.CompletedTask;
        MessageQueue.Enqueue(message);
    }
}