using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitListener.Core.Services;

public class QueueReceiver
{
    private readonly IModel _channel;

    public QueueReceiver()
    {
        _channel = new ConnectionFactory { HostName = "localhost"}
            .CreateConnection().CreateModel();
    }
    
    public void StartListening(string queueName)
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentNullException(nameof(queueName));
        }
        
        // _channel.QueueDeclare(queue: queueName,
        //     durable: false,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await SendMessageToQueue(message);

            // Acknowledge the message
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: queueName,
            autoAck: false,  // Manual acknowledgment
            consumer: consumer);
    }

    private static async Task SendMessageToQueue(string message)
    {
        await Task.CompletedTask;
        QueueReader.MessageQueue.Enqueue(message);
    }
}