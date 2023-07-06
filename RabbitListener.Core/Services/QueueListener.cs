using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitListener.Core.Services;

public class QueueListener
{
    public Action<string>? OnMessageReceived;
    
    private readonly IModel _channel;

    public QueueListener()
    {
        var inDocker = bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out _);

            _channel = new ConnectionFactory
            {
                HostName = inDocker ? "host.docker.internal" : "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            }
            .CreateConnection().CreateModel();
    }
    
    public void StartListening(string queueName = "urls")
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentNullException(queueName);
        }

        try
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                OnMessageReceived?.Invoke(message);

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: queueName,
                autoAck: false,
                consumer: consumer);
        }
        catch (Exception e)
        {
            throw new Exception();
        }
    }
}
