using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitListener.Core.Services;

public class QueueListener
{
    private readonly IModel _channel;

    public QueueListener()
    {
        var inDocker = bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var _);

            _channel = new ConnectionFactory
            {
                HostName = inDocker ? "host.docker.internal" : "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            }
            .CreateConnection().CreateModel();
    }
    
    public bool TryStartListening(string queueName)
    {
        if (string.IsNullOrWhiteSpace(queueName)) return false;
        
        try
        {
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
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}