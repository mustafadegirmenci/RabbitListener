using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitListener.Core.Services;

public class QueueListener
{
    private readonly IModel _channel;

    public QueueListener()
    {
        _channel = new ConnectionFactory { HostName = "localhost"}
            .CreateConnection().CreateModel();
    }
    
    public bool TryStartListening(string queueName)
    {
        if (string.IsNullOrWhiteSpace(queueName)) return false;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await QueueManager.FetchMessage(message);

            // Acknowledge the message
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: queueName,
            autoAck: false,  // Manual acknowledgment
            consumer: consumer);

        return true;
    }
}