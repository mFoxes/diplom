using System.Text;
using DTO;
using GrandmaApi.Models.Configs;
using System.Text.Json;
using GrandmaApi.Mediatr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GrandmaApi.RabbitMqBus;

public class RabbitMqMessageService : IRabbitMqMessageService, IDisposable
{
    private readonly IOptions<RabbitMqConfig> _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    
    public RabbitMqMessageService(IOptions<RabbitMqConfig> config)
    {
        _config = config;
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_config.Value.ConnectionUri)
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
    }
    public void SendMessage<T>(T message, MessageTypes messageType)
    {
        var queueName = DefineQueue(messageType);
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false,
            autoDelete: false, arguments: null);
        var props = _channel.CreateBasicProperties();
        props.Type = MessageTypeConverter.FromType(messageType);
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "", routingKey:queueName, basicProperties:props, body: body);
    }

    private string DefineQueue(MessageTypes type)
    {
        var configValue = _config.Value;
        return type == MessageTypes.EmbeddingsUpdate ? configValue.GrandmaNamesQueue : configValue.GrandmaPersonsQueue;
    }
    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}